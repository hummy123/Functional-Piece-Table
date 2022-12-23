``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  Job-GNCVCO : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
|           Method |  size |        Mean |      Error |     StdDev |      Median |
|----------------- |------ |------------:|-----------:|-----------:|------------:|
| **ConsolidateTable** |   **100** |    **55.18 μs** |   **1.207 μs** |   **3.444 μs** |    **53.80 μs** |
| **ConsolidateTable** |  **1000** |   **602.80 μs** |  **11.995 μs** |  **22.530 μs** |   **597.55 μs** |
| **ConsolidateTable** | **10000** | **5,223.30 μs** | **110.854 μs** | **314.474 μs** | **5,158.00 μs** |
