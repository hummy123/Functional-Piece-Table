``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-QPARYZ : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
|                 Method |  size |     Mean |     Error |    StdDev |   Median |
|----------------------- |------ |---------:|----------:|----------:|---------:|
| **InsertNearStartOfTable** |   **100** | **3.583 μs** | **0.3264 μs** | **0.9314 μs** | **3.400 μs** |
|  InsertAtMiddleOfTable |   100 | 2.623 μs | 0.1398 μs | 0.4033 μs | 2.500 μs |
|   InsertNearEndOfTable |   100 | 2.856 μs | 0.3177 μs | 0.8644 μs | 2.500 μs |
| **InsertNearStartOfTable** |  **1000** | **4.264 μs** | **0.3216 μs** | **0.9069 μs** | **3.900 μs** |
|  InsertAtMiddleOfTable |  1000 | 4.437 μs | 0.3848 μs | 1.0791 μs | 4.000 μs |
|   InsertNearEndOfTable |  1000 | 5.286 μs | 0.7025 μs | 1.9814 μs | 4.400 μs |
| **InsertNearStartOfTable** | **10000** | **4.821 μs** | **0.3225 μs** | **0.9406 μs** | **4.600 μs** |
|  InsertAtMiddleOfTable | 10000 | 4.858 μs | 0.2850 μs | 0.7850 μs | 4.800 μs |
|   InsertNearEndOfTable | 10000 | 4.899 μs | 0.3883 μs | 1.0825 μs | 4.700 μs |
