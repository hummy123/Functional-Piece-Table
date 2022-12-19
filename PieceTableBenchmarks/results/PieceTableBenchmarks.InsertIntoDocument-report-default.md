
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-JUFKHE : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |      Mean |     Error |    StdDev |    Median | Allocated |
------------------------ |------------ |----------:|----------:|----------:|----------:|----------:|
  **InsertIntoTableAtStart** |         **100** |  **4.452 μs** | **0.7724 μs** | **2.1913 μs** |  **3.700 μs** |   **1.84 KB** |
 InsertIntoTableAtMiddle |         100 |  3.819 μs | 0.4189 μs | 1.1745 μs |  3.300 μs |   1.84 KB |
    InsertIntoTableAtEnd |         100 |  3.393 μs | 0.3150 μs | 0.8516 μs |  3.100 μs |   1.84 KB |
  **InsertIntoTableAtStart** |        **1000** | **11.199 μs** | **1.7656 μs** | **5.1781 μs** | **11.400 μs** |   **3.09 KB** |
 InsertIntoTableAtMiddle |        1000 | 13.106 μs | 2.0057 μs | 5.9139 μs | 12.900 μs |   3.09 KB |
    InsertIntoTableAtEnd |        1000 | 12.924 μs | 1.6731 μs | 4.8538 μs | 12.750 μs |   3.09 KB |
  **InsertIntoTableAtStart** |       **10000** | **12.944 μs** | **1.0899 μs** | **3.0018 μs** | **12.650 μs** |   **3.27 KB** |
 InsertIntoTableAtMiddle |       10000 | 11.097 μs | 1.0743 μs | 3.0651 μs |  9.900 μs |   3.59 KB |
    InsertIntoTableAtEnd |       10000 | 10.582 μs | 0.7600 μs | 2.0931 μs |  9.600 μs |   3.59 KB |
