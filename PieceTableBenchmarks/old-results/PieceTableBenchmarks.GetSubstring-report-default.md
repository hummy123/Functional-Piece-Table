
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-IWUGBN : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |       Mean |      Error |     StdDev |     Median | Allocated |
---------------------------- |------------ |-----------:|-----------:|-----------:|-----------:|----------:|
  **GetSubstringAtStartOfTable** |         **100** |   **2.900 μs** |  **0.1644 μs** |  **0.4636 μs** |   **2.800 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |         100 |   6.113 μs |  0.3092 μs |  0.8871 μs |   5.900 μs |    5904 B |
    GetSubstringAtEndOfTable |         100 |   8.205 μs |  0.1678 μs |  0.4450 μs |   8.100 μs |    7912 B |
  **GetSubstringAtStartOfTable** |        **1000** |   **5.076 μs** |  **0.3080 μs** |  **0.8688 μs** |   **4.900 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |        1000 |  41.315 μs |  1.0180 μs |  2.9044 μs |  40.500 μs |   55024 B |
    GetSubstringAtEndOfTable |        1000 |  59.646 μs |  1.1209 μs |  1.4575 μs |  59.600 μs |   72712 B |
  **GetSubstringAtStartOfTable** |       **10000** |   **5.364 μs** |  **0.5598 μs** |  **1.5788 μs** |   **5.100 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |       10000 | 187.544 μs | 16.3854 μs | 45.1302 μs | 167.400 μs | 1078080 B |
    GetSubstringAtEndOfTable |       10000 | 236.109 μs |  4.6841 μs | 11.2228 μs | 231.450 μs |  720712 B |
