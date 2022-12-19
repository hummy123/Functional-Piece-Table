
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-JUFKHE : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |       Mean |     Error |     StdDev |     Median | Allocated |
------------------------ |------------ |-----------:|----------:|-----------:|-----------:|----------:|
  **DeleteFromStartOfTable** |         **100** |   **2.436 μs** | **0.1329 μs** |  **0.3703 μs** |   **2.400 μs** |     **792 B** |
 DeleteFromMiddleOfTable |         100 |   7.111 μs | 0.2393 μs |  0.6470 μs |   7.200 μs |    9320 B |
    DeleteFromEndOfTable |         100 |  10.761 μs | 0.2599 μs |  0.7414 μs |  10.700 μs |   17384 B |
  **DeleteFromStartOfTable** |        **1000** |   **5.561 μs** | **0.4810 μs** |  **1.3409 μs** |   **5.400 μs** |     **792 B** |
 DeleteFromMiddleOfTable |        1000 |  43.132 μs | 0.8621 μs |  2.3160 μs |  42.400 μs |   84920 B |
    DeleteFromEndOfTable |        1000 |  81.410 μs | 1.6042 μs |  2.9735 μs |  80.350 μs |  168584 B |
  **DeleteFromStartOfTable** |       **10000** |   **4.899 μs** | **0.7039 μs** |  **2.0420 μs** |   **4.500 μs** |     **792 B** |
 DeleteFromMiddleOfTable |       10000 | 221.841 μs | 3.7400 μs |  5.8227 μs | 221.950 μs |  840920 B |
    DeleteFromEndOfTable |       10000 | 454.928 μs | 9.0652 μs | 20.2757 μs | 450.300 μs | 1680584 B |
