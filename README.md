# Purely Functional Piece Table in F#

## Introduction
This is a purely functional Piece Table written in F#. A Piece Table is a data structure that has often been used in text editors such as Visual Studio/Code and AbiWord. 

For more information about the Piece Table data structure, [this repository](https://github.com/veler/Csharp-Piece-Table-Implementation) provides some helpful introductory links.

This branch contains a Piece Table with the Pieces stored in a linked list with a zipper which, like a splay tree, moves us to the node where the last operation was performed. This lets us reach nodes local to the previous one and perform operations on them more efficiently.

After [benchmarking number-insert operations on a zipper and a balanced binary tree](https://github.com/hummy123/FunctionStructureComparison), I began to be be less impressed with the constant time operations zippers can potentially have and saw the good (close to a zipper's best in all cases) performance of a red black tree as a reason to switch.

## Insert Benchmarks

### Piece Table with ListZipper

|                 Method | size |       Mean |     Error |    StdDev |     Median | Allocated |
|----------------------- |----- |-----------:|----------:|----------:|-----------:|----------:|
|   **InsertNearEndOfTable** |  **100** |  **15.537 μs** | **0.6592 μs** | **1.8806 μs** |  **14.800 μs** |   **8.89 KB** |
| InsertNearStartOfTable |  100 |   2.906 μs | 0.1870 μs | 0.5149 μs |   2.800 μs |   1.95 KB |
|  InsertAtMiddleOfTable |  100 |  10.586 μs | 0.3892 μs | 1.0976 μs |  10.200 μs |   5.47 KB |
|   **InsertNearEndOfTable** | **1000** | **898.265 μs** | **6.2429 μs** | **5.2131 μs** | **899.350 μs** |  **80.96 KB** |
| InsertNearStartOfTable | 1000 |   5.707 μs | 0.2944 μs | 0.8159 μs |   5.500 μs |  10.74 KB |
|  InsertAtMiddleOfTable | 1000 | 725.543 μs | 8.7436 μs | 7.7510 μs | 726.400 μs |   45.9 KB |

#### Plain Piece Table (standard linked list) implementation

|                 Method | size |         Mean |       Error |     StdDev |       Median |      Gen0 |   Allocated |
|----------------------- |----- |-------------:|------------:|-----------:|-------------:|----------:|------------:|
|   **InsertNearEndOfTable** |  **100** |   **137.018 μs** |   **7.2559 μs** |  **21.394 μs** |   **127.550 μs** |         **-** |   **162.77 KB** |
| InsertNearStartOfTable |  100 |     5.311 μs |   0.4859 μs |   1.386 μs |     4.950 μs |         - |     1.95 KB |
|  InsertAtMiddleOfTable |  100 |    35.397 μs |   0.6986 μs |   1.108 μs |    35.700 μs |         - |    43.35 KB |
|   **InsertNearEndOfTable** | **1000** | **9,133.143 μs** | **182.6068 μs** | **474.619 μs** | **9,059.000 μs** | **7000.0000** | **15682.49 KB** |
| InsertNearStartOfTable | 1000 |     7.935 μs |   0.6310 μs |   1.790 μs |     7.300 μs |         - |    10.73 KB |
|  InsertAtMiddleOfTable | 1000 | 2,646.246 μs |  95.5599 μs | 274.179 μs | 2,622.500 μs | 1000.0000 |  3940.42 KB |

### Benchmark Hardware

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1219/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.402
  [Host]     : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2 DEBUG
  Job-ARMIDF : .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  
