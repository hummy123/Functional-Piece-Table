
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-JUFKHE : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |       Mean |     Error |     StdDev |     Median | Allocated |
---------------------------- |------------ |-----------:|----------:|-----------:|-----------:|----------:|
  **GetSubstringAtStartOfTable** |         **100** |   **3.855 μs** | **0.2772 μs** |  **0.7865 μs** |   **3.600 μs** |     **880 B** |
 GetSubstringAtMiddleOfTable |         100 |  10.639 μs | 1.3362 μs |  3.8978 μs |  10.150 μs |    6048 B |
    GetSubstringAtEndOfTable |         100 |   9.096 μs | 0.2924 μs |  0.8150 μs |   8.950 μs |    8056 B |
  **GetSubstringAtStartOfTable** |        **1000** |   **7.210 μs** | **0.7746 μs** |  **2.2595 μs** |   **6.000 μs** |    **1096 B** |
 GetSubstringAtMiddleOfTable |        1000 |  45.692 μs | 1.8987 μs |  5.3242 μs |  45.400 μs |   55552 B |
    GetSubstringAtEndOfTable |        1000 |  62.221 μs | 2.0764 μs |  5.8566 μs |  61.200 μs |   72856 B |
  **GetSubstringAtStartOfTable** |       **10000** |   **8.328 μs** | **0.9348 μs** |  **2.7564 μs** |   **7.600 μs** |    **1096 B** |
 GetSubstringAtMiddleOfTable |       10000 | 171.584 μs | 9.4421 μs | 25.8476 μs | 159.900 μs | 1081248 B |
    GetSubstringAtEndOfTable |       10000 | 217.437 μs | 5.9417 μs | 16.4645 μs | 212.700 μs |  720856 B |
