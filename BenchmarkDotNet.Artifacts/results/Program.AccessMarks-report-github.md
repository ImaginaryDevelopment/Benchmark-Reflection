``` ini

BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.5 (18F203) [Darwin 18.6.0]
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.203
  [Host]       : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT DEBUG
  NetCoreApp21 : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT

Job=NetCoreApp21  Runtime=Core  Toolchain=.NET Core 2.1  
IterationCount=15  LaunchCount=2  WarmupCount=10  

```
|         Method | sleepTime |          Mean |         Error |        StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------- |---------- |--------------:|--------------:|--------------:|-------:|------:|------:|----------:|
|      **RunPublic** |         **0** |      **42.88 us** |     **1.3946 us** |     **2.0441 us** | **0.9766** |     **-** |     **-** |   **3.72 KB** |
|        Reflect |         0 |      48.04 us |     0.5617 us |     0.8407 us | 1.0376 |     - |     - |   3.81 KB |
| DynamicReflect |         0 |      48.58 us |     0.8363 us |     1.2517 us | 1.0376 |     - |     - |   3.94 KB |
|      **RunPublic** |         **1** |   **5,488.67 us** |    **27.4092 us** |    **41.0249 us** |      **-** |     **-** |     **-** |   **3.91 KB** |
|        Reflect |         1 |   5,543.29 us |    23.8211 us |    34.9166 us |      - |     - |     - |   4.02 KB |
| DynamicReflect |         1 |   5,550.47 us |    34.0464 us |    50.9591 us |      - |     - |     - |   4.15 KB |
|      **RunPublic** |        **15** |  **68,756.56 us** | **1,272.6236 us** | **1,904.8031 us** |      **-** |     **-** |     **-** |   **3.91 KB** |
|        Reflect |        15 |  75,801.20 us | 1,212.3736 us | 1,814.6238 us |      - |     - |     - |   4.24 KB |
| DynamicReflect |        15 |  74,880.26 us | 1,380.9247 us | 2,066.9031 us |      - |     - |     - |   4.38 KB |
|      **RunPublic** |       **100** | **405,453.18 us** | **2,731.6902 us** | **4,004.0760 us** |      **-** |     **-** |     **-** |   **3.91 KB** |
|        Reflect |       100 | 406,637.86 us | 2,962.5000 us | 4,434.1305 us |      - |     - |     - |   5.66 KB |
| DynamicReflect |       100 | 407,360.33 us | 2,343.5643 us | 3,507.7367 us |      - |     - |     - |   5.88 KB |
