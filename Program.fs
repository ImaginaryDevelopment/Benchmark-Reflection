open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Diagnosers
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Running
open BenchmarkDotNet.Validators
open BenchmarkDotNet.Exporters
open BenchmarkDotNet.Environments
open System.Reflection
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Toolchains.CsProj
open BenchmarkDotNet.Toolchains.DotNetCli

type Publics(sleepTime:int) =
    member this.Thread() = System.Threading.Thread.Sleep(sleepTime)
    member this.Task() = System.Threading.Tasks.Task.Delay(sleepTime) |> Async.AwaitTask |> Async.RunSynchronously
    member this.AsyncToTask () = Async.Sleep(sleepTime) |> Async.StartAsTask |> Async.AwaitTask |> Async.RunSynchronously
    member this.AsyncToSync () = Async.Sleep(sleepTime) |> Async.RunSynchronously

// type Internals(sleepTime:int) =
//     member internal this.Thread() = System.Threading.Thread.Sleep(sleepTime)
//     member internal this.Task() = System.Threading.Tasks.Task.Delay(sleepTime) |> Async.AwaitTask |> Async.RunSynchronously
//     member internal this.AsyncToTask () = Async.Sleep(sleepTime) |> Async.StartAsTask |> Async.AwaitTask |> Async.RunSynchronously
//     member internal this.AsyncToSync () = Async.Sleep(sleepTime) |> Async.RunSynchronously

// type Privates(sleepTime:int) =
//     member private this.Thread() = System.Threading.Thread.Sleep(sleepTime)
//     member private this.Task() = System.Threading.Tasks.Task.Delay(sleepTime) |> Async.AwaitTask |> Async.RunSynchronously
//     member private this.AsyncToTask () = Async.Sleep(sleepTime) |> Async.StartAsTask |> Async.AwaitTask |> Async.RunSynchronously
//     member private this.AsyncToSync () = Async.Sleep(sleepTime) |> Async.RunSynchronously
module PublicMethodAccess =
    open Microsoft.FSharp.Reflection
    let (?) (this : 'Source) (member' : string) (args : 'Args) : 'Result =
        let argArray =
            if isNull <| box args then null
            elif FSharpType.IsTuple (args.GetType()) then
              FSharpValue.GetTupleFields args
            else [|args|]

        let flags = BindingFlags.GetProperty ||| BindingFlags.InvokeMethod
        this.GetType().InvokeMember(member', flags, null, this, argArray) :?> 'Result
    // let runMethodSample() =
    //     x?GetCode()
    // let runMethodArgSample() =
    //     x?GetCode(1)
open PublicMethodAccess

type AccessMarks() =
    [<Params(0, 1, 15, 100)>]
    member val public sleepTime = 0 with get, set

    // [<GlobalSetup>]
    // member self.GlobalSetup() =
    //     printfn "%s" "Global Setup"

    // [<GlobalCleanup>]
    // member self.GlobalCleanup() =
    //     printfn "%s" "Global Cleanup"

    // [<IterationSetup>]
    // member self.IterationSetup() =
    //     printfn "%s" "Iteration Setup"
    // [<IterationCleanup>]
    // member self.IterationCleanup() =
    //     printfn "%s" "Iteration Cleanup"

    [<Benchmark>]
    member this.RunPublic() =
      let p = Publics(this.sleepTime)
      p.Thread()
      p.Task()
      p.AsyncToSync()
      p.AsyncToTask()
    // member this.RunInternalDirect() =
    //   let p = Internals(this.sleepTime)
    //   p.Thread()
    //   p.Task()
    //   p.AsyncToSync()
    //   p.AsyncToTask()
    // [<BenchMark>]
    // member this.RunInternalReflect() =
    //   let p:obj= Internals(this.sle)
    [<Benchmark>]
    member this.Reflect():unit =
      let p:obj = upcast Publics(this.sleepTime)
      let t = p.GetType()
      t.GetMethod("Thread").Invoke(p,Array.empty) |> ignore
      t.GetMethod("Task").Invoke(p,Array.empty) |> ignore
      t.GetMethod("AsyncToSync").Invoke(p, Array.empty) |> ignore
      t.GetMethod("AsyncToTask").Invoke(p, Array.empty) |> ignore
    [<Benchmark>]
    member this.DynamicReflect():unit =
      let p:obj = upcast Publics(this.sleepTime)
      let t = p.GetType()
      p?Thread()
      p?Task()
      p?AsyncToSync()
      p?AsyncToTask()
      // t.GetMethod("Thread").Invoke(p,Array.empty) |> ignore
      // t.GetMethod("Task").Invoke(p,Array.empty) |> ignore
      // t.GetMethod("AsyncToSync").Invoke(p, Array.empty) |> ignore
      // t.GetMethod("AsyncToTask").Invoke(p, Array.empty) |> ignore

    // [<Benchmark>]
    // member this.Task () = System.Threading.Tasks.Task.Delay(this.sleepTime)

    // [<Benchmark>]
    // member this.AsyncToTask () = Async.Sleep(this.sleepTime) |> Async.StartAsTask
    // [<Benchmark>]
    // member this.AsyncToSync () = Async.Sleep(this.sleepTime) |> Async.RunSynchronously


let config =
     ManualConfig
            .Create(DefaultConfig.Instance)
            // .With(Job.ShortRun.With(Runtime.Core))
            .With(Job.MediumRun
                .With(Runtime.Core)
                .With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21))
                .WithId("NetCoreApp21")
            )
            .With(MemoryDiagnoser.Default)
            .With(MarkdownExporter.GitHub)
            .With(ExecutionValidator.FailOnError)

let defaultSwitch () =
    Assembly.GetExecutingAssembly().GetTypes() |> Array.filter (fun t ->
        t.GetMethods ()|> Array.exists (fun m ->
            m.GetCustomAttributes (typeof<BenchmarkAttribute>, false) <> [||] ))
    |> BenchmarkSwitcher


[<EntryPoint>]
let main argv =
    // GiraffeMarks().Uncached()
    defaultSwitch().Run(argv, config) |>ignore
    // BenchmarkRunner.Run<SleepMarks>(config) |> ignore
    0 // return an integer exit code
