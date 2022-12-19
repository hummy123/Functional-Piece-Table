
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-UNRPTJ : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                 Method |  size |     Mean |     Error |    StdDev |   Median | Allocated |
----------------------- |------ |---------:|----------:|----------:|---------:|----------:|
 **InsertNearStartOfTable** |   **100** | **7.292 μs** | **0.8456 μs** | **2.4532 μs** | **6.800 μs** |     **976 B** |
  InsertAtMiddleOfTable |   100 | 4.590 μs | 0.3367 μs | 0.9386 μs | 4.350 μs |     880 B |
   InsertNearEndOfTable |   100 | 5.276 μs | 0.6299 μs | 1.7870 μs | 4.900 μs |     880 B |
 **InsertNearStartOfTable** |  **1000** | **5.265 μs** | **0.6833 μs** | **1.9715 μs** | **4.400 μs** |    **1288 B** |
  InsertAtMiddleOfTable |  1000 | 5.217 μs | 0.5835 μs | 1.6552 μs | 4.400 μs |    1192 B |
   InsertNearEndOfTable |  1000 | 4.942 μs | 0.5526 μs | 1.5404 μs | 4.400 μs |    1192 B |
 **InsertNearStartOfTable** | **10000** | **5.545 μs** | **0.6095 μs** | **1.7584 μs** | **4.950 μs** |    **1472 B** |
  InsertAtMiddleOfTable | 10000 | 5.347 μs | 0.5059 μs | 1.4597 μs | 5.100 μs |    1376 B |
   InsertNearEndOfTable | 10000 | 5.019 μs | 0.5254 μs | 1.5075 μs | 4.500 μs |    1376 B |
