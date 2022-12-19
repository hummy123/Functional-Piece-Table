
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2


                 Method | stringLength |        Mean |     Error |    StdDev |    Gen0 | Allocated |
----------------------- |------------- |------------:|----------:|----------:|--------:|----------:|
 **CreatePieceTableOfSize** |          **100** |    **89.97 ns** |  **0.579 ns** |  **0.541 ns** |  **0.1988** |     **416 B** |
 **CreatePieceTableOfSize** |         **1000** |   **287.90 ns** |  **5.707 ns** |  **6.793 ns** |  **1.0591** |    **2216 B** |
 **CreatePieceTableOfSize** |        **10000** | **5,739.83 ns** | **88.127 ns** | **78.123 ns** | **29.2664** |   **61456 B** |
