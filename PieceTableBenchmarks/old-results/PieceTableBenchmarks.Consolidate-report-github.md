``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-IWUGBN : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
|           Method |  size |        Mean |      Error |     StdDev |      Median |
|----------------- |------ |------------:|-----------:|-----------:|------------:|
| **ConsolidateTable** |   **100** |    **25.88 μs** |   **1.003 μs** |   **2.764 μs** |    **24.80 μs** |
| **ConsolidateTable** |  **1000** |   **328.74 μs** |   **6.533 μs** |  **17.324 μs** |   **325.40 μs** |
| **ConsolidateTable** | **10000** | **3,397.61 μs** | **142.296 μs** | **408.274 μs** | **3,326.00 μs** |
