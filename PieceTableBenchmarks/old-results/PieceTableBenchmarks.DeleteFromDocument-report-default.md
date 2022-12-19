
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-IWUGBN : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                  Method | insertTimes |       Mean |      Error |      StdDev |     Median | Allocated |
------------------------ |------------ |-----------:|-----------:|------------:|-----------:|----------:|
  **DeleteFromStartOfTable** |         **100** |   **2.815 μs** |  **0.2469 μs** |   **0.6922 μs** |   **2.650 μs** |     **792 B** |
 DeleteFromMiddleOfTable |         100 |   8.001 μs |  0.4405 μs |   1.2710 μs |   7.600 μs |    9320 B |
    DeleteFromEndOfTable |         100 |  10.510 μs |  0.2350 μs |   0.6550 μs |  10.400 μs |   17384 B |
  **DeleteFromStartOfTable** |        **1000** |   **4.219 μs** |  **0.2596 μs** |   **0.7407 μs** |   **4.150 μs** |     **792 B** |
 DeleteFromMiddleOfTable |        1000 |  40.847 μs |  0.7002 μs |   1.0901 μs |  40.600 μs |   84920 B |
    DeleteFromEndOfTable |        1000 |  78.420 μs |  1.5332 μs |   2.0467 μs |  78.300 μs |  168584 B |
  **DeleteFromStartOfTable** |       **10000** |   **5.172 μs** |  **0.5671 μs** |   **1.6088 μs** |   **4.700 μs** |     **792 B** |
 DeleteFromMiddleOfTable |       10000 | 380.046 μs |  6.9798 μs |   5.8285 μs | 380.500 μs |  840920 B |
    DeleteFromEndOfTable |       10000 | 546.139 μs | 56.5478 μs | 164.0554 μs | 460.700 μs | 1680584 B |
