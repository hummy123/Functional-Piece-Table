``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-JUFKHE : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
|           Method |  size |         Mean |      Error |     StdDev |
|----------------- |------ |-------------:|-----------:|-----------:|
| **ConsolidateTable** |   **100** |     **43.28 μs** |   **1.849 μs** |   **5.061 μs** |
| **ConsolidateTable** |  **1000** |    **975.02 μs** |  **19.340 μs** |  **17.144 μs** |
| **ConsolidateTable** | **10000** | **11,339.06 μs** | **193.674 μs** | **358.987 μs** |
