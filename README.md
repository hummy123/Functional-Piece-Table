# Purely Functional Piece Table in F#

## Introduction
This is a purely functional Piece Table written in F#. A Piece Table is a data structure that has often been used in text editors such as Visual Studio/Code and AbiWord. 

For more information about the Piece Table data structure, [this repository](https://github.com/veler/Csharp-Piece-Table-Implementation) provides some helpful introductory links.

This branch contains a Piece Table with the Pieces stored in a linked list with a zipper which, like a splay tree, moves us to the node where the last operation was performed. This lets us reach nodes local to the previous one and perform operations on them more efficiently.

## Insert Benchmarks

### Analysis

#### From standard linked list to linked list with zipper

There is overall a significant reduction in the time and allocated memory with the zipper. When we have a size (initial number of pieces) of 100 in the table, inserting at the start is about a 60% time reduction, 75% for inserting at the middle and 89% for inserting at the end.

When we have a size of 1000, inserting near the start of the table actually takes about 18% (3 μs) more time. This is likely because of the iteration setup we use to fill the table with pieces in the first place. We set up by inserting pieces at the end of the table repeatedly (up until the size parameter is reached), which means our zipper is naturally close to the end of the table by the time set setup is finished. However, it is a curious fact - and possibly an indicator of volatile speed - that inserting near the start in the other cases results in a speed up.

There are 72% (insertion at middle) and 91% (insertion at end) speed improvements for a size of 1000 as well. When our table has 10,000 pieces, insertion at start has a 28% improvement, insertion at the middle 73% and insertion at the end 90%.

This is overall an improvement over the standard linked list implementation. The performance of a zipper depends on where the node we want to edit is and the zipper's current focus, so edits further away from the zipper's current focus (uncommon in text editors) take longer while edits close to the zipper's current focus (the usual case in text editors) are faster.

There is also less garbage produced and thus less need for the garbage collector to step in and collect it with my zipper implementation, but this is almost certainly due to the way I implemented the plain linked list version. For that plain linked list version, we iterate (through recursion) over each piece in the list and use an accumulator to rebuild the list with the pieces at the desired location, and it is this rebuilding that taxes the GC more in that implementation. 

This could have been avoided by iterating over the list to see which list index (as opposed to string/character index) we want to insert at and then using the List.insert method provided by F#, although this may have a higher time complexity (iterating to the same position twice, once for the search and one for the insertion). I believe the splitting logic (when we want to insert "cd" into the string "abef" to form "abcdef") is also simpler in the implementation I provided. 

### Raw data

#### Piece Table with ListZipper

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
