
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2


                 Method | stringLength |       Mean |     Error |    StdDev |    Gen0 | Allocated |
----------------------- |------------- |-----------:|----------:|----------:|--------:|----------:|
 **CreatePieceTableOfSize** |          **100** |   **2.291 μs** | **0.0452 μs** | **0.0520 μs** |  **0.4272** |     **896 B** |
 **CreatePieceTableOfSize** |         **1000** |  **26.902 μs** | **0.3021 μs** | **0.2523 μs** |  **2.9907** |    **6296 B** |
 **CreatePieceTableOfSize** |        **10000** | **611.929 μs** | **6.0420 μs** | **5.0453 μs** | **83.0078** |  **178449 B** |
