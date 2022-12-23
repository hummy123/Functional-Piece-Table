
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  Job-GNCVCO : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |     Mean |     Error |    StdDev |   Median | Allocated |
------------------------ |------------ |---------:|----------:|----------:|---------:|----------:|
  **InsertIntoTableAtStart** |         **100** | **7.735 μs** | **1.4873 μs** | **4.3150 μs** | **6.100 μs** |   **1.78 KB** |
 InsertIntoTableAtMiddle |         100 | 8.465 μs | 1.4433 μs | 4.2329 μs | 7.500 μs |   1.78 KB |
    InsertIntoTableAtEnd |         100 | 5.397 μs | 0.7692 μs | 2.1821 μs | 4.400 μs |   1.78 KB |
  **InsertIntoTableAtStart** |        **1000** | **5.090 μs** | **0.3311 μs** | **0.8837 μs** | **4.800 μs** |   **2.68 KB** |
 InsertIntoTableAtMiddle |        1000 | 6.720 μs | 0.8221 μs | 2.3981 μs | 5.600 μs |   2.68 KB |
    InsertIntoTableAtEnd |        1000 | 6.654 μs | 0.9400 μs | 2.7122 μs | 5.300 μs |   2.68 KB |
  **InsertIntoTableAtStart** |       **10000** | **7.196 μs** | **0.8109 μs** | **2.3135 μs** | **6.550 μs** |   **2.79 KB** |
 InsertIntoTableAtMiddle |       10000 | 7.634 μs | 0.9494 μs | 2.7394 μs | 6.850 μs |   2.79 KB |
    InsertIntoTableAtEnd |       10000 | 8.905 μs | 1.2695 μs | 3.6425 μs | 8.300 μs |   2.79 KB |
