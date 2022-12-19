
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-IWUGBN : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |     Mean |     Error |    StdDev |   Median | Allocated |
------------------------ |------------ |---------:|----------:|----------:|---------:|----------:|
  **InsertIntoTableAtStart** |         **100** | **4.443 μs** | **0.5906 μs** | **1.7133 μs** | **4.100 μs** |   **1.67 KB** |
 InsertIntoTableAtMiddle |         100 | 2.843 μs | 0.2494 μs | 0.6953 μs | 2.550 μs |   1.67 KB |
    InsertIntoTableAtEnd |         100 | 3.162 μs | 0.2843 μs | 0.8066 μs | 2.900 μs |   1.67 KB |
  **InsertIntoTableAtStart** |        **1000** | **4.692 μs** | **0.4197 μs** | **1.1768 μs** | **4.500 μs** |   **2.57 KB** |
 InsertIntoTableAtMiddle |        1000 | 4.095 μs | 0.2657 μs | 0.7539 μs | 3.900 μs |   2.57 KB |
    InsertIntoTableAtEnd |        1000 | 4.805 μs | 0.3312 μs | 0.9287 μs | 4.500 μs |   2.57 KB |
  **InsertIntoTableAtStart** |       **10000** | **4.416 μs** | **0.4599 μs** | **1.3046 μs** | **4.100 μs** |   **2.68 KB** |
 InsertIntoTableAtMiddle |       10000 | 5.233 μs | 0.7112 μs | 2.0520 μs | 4.700 μs |   2.68 KB |
    InsertIntoTableAtEnd |       10000 | 4.382 μs | 0.4338 μs | 1.2092 μs | 4.000 μs |   2.68 KB |
