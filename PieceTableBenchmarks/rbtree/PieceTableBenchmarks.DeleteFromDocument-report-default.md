
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  Job-GNCVCO : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |       Mean |     Error |     StdDev |     Median | Allocated |
------------------------ |------------ |-----------:|----------:|-----------:|-----------:|----------:|
  **DeleteFromStartOfTable** |         **100** |   **3.279 μs** | **0.3746 μs** |  **1.0441 μs** |   **2.850 μs** |     **792 B** |
 DeleteFromMiddleOfTable |         100 |   9.823 μs | 0.7246 μs |  2.1136 μs |   9.350 μs |    9320 B |
    DeleteFromEndOfTable |         100 |  10.608 μs | 0.2157 μs |  0.5085 μs |  10.650 μs |   17384 B |
  **DeleteFromStartOfTable** |        **1000** |   **3.273 μs** | **0.1071 μs** |  **0.2897 μs** |   **3.200 μs** |     **792 B** |
 DeleteFromMiddleOfTable |        1000 |  38.146 μs | 0.7262 μs |  0.6064 μs |  38.100 μs |   84920 B |
    DeleteFromEndOfTable |        1000 |  75.850 μs | 1.3779 μs |  1.2214 μs |  75.600 μs |  168584 B |
  **DeleteFromStartOfTable** |       **10000** |   **5.395 μs** | **0.8129 μs** |  **2.3454 μs** |   **4.700 μs** |     **792 B** |
 DeleteFromMiddleOfTable |       10000 | 194.735 μs | 9.7615 μs | 26.2235 μs | 184.150 μs |  840920 B |
    DeleteFromEndOfTable |       10000 | 436.332 μs | 8.4877 μs | 21.2939 μs | 431.800 μs | 1680584 B |
