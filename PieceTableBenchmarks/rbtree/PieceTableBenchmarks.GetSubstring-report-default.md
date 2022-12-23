
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  Job-GNCVCO : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |       Mean |      Error |     StdDev |     Median | Allocated |
---------------------------- |------------ |-----------:|-----------:|-----------:|-----------:|----------:|
  **GetSubstringAtStartOfTable** |         **100** |   **3.052 μs** |  **0.1884 μs** |  **0.5093 μs** |   **2.900 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |         100 |   6.826 μs |  0.5571 μs |  1.5894 μs |   6.200 μs |    5904 B |
    GetSubstringAtEndOfTable |         100 |   7.449 μs |  0.1524 μs |  0.2589 μs |   7.500 μs |    7912 B |
  **GetSubstringAtStartOfTable** |        **1000** |   **4.461 μs** |  **0.4364 μs** |  **1.2452 μs** |   **3.950 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |        1000 |  37.248 μs |  1.0329 μs |  2.8450 μs |  36.000 μs |   55024 B |
    GetSubstringAtEndOfTable |        1000 |  56.450 μs |  1.1282 μs |  1.8849 μs |  55.600 μs |   72712 B |
  **GetSubstringAtStartOfTable** |       **10000** |   **5.000 μs** |  **0.5267 μs** |  **1.4856 μs** |   **4.500 μs** |     **736 B** |
 GetSubstringAtMiddleOfTable |       10000 | 198.160 μs | 16.3797 μs | 46.1994 μs | 173.400 μs | 1078080 B |
    GetSubstringAtEndOfTable |       10000 | 194.930 μs |  3.9045 μs | 10.4892 μs | 191.200 μs |  720712 B |
