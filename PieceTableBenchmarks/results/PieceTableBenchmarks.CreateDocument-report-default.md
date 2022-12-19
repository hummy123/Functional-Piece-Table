
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2


                 Method | stringLength |       Mean |     Error |    StdDev |    Gen0 | Allocated |
----------------------- |------------- |-----------:|----------:|----------:|--------:|----------:|
 **CreatePieceTableOfSize** |          **100** |   **121.7 ns** |   **2.24 ns** |   **2.58 ns** |  **0.2141** |     **448 B** |
 **CreatePieceTableOfSize** |         **1000** |   **334.0 ns** |   **6.75 ns** |  **16.05 ns** |  **1.0738** |    **2248 B** |
 **CreatePieceTableOfSize** |        **10000** | **5,747.7 ns** | **113.64 ns** | **180.25 ns** | **20.7443** |   **43528 B** |
