
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2 DEBUG
  Job-GNCVCO : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |     Mean |     Error |    StdDev |   Median | Allocated |
---------------------------- |------------ |---------:|----------:|----------:|---------:|----------:|
      **InsertIntoTableAtStart** |         **100** | **3.618 μs** | **0.1135 μs** | **0.3200 μs** | **3.600 μs** |    **1824 B** |
     InsertIntoTableAtMiddle |         100 | 4.213 μs | 0.2971 μs | 0.8619 μs | 4.000 μs |    1920 B |
        InsertIntoTableAtEnd |         100 | 3.300 μs | 0.0699 μs | 0.1128 μs | 3.300 μs |    1824 B |
      DeleteFromStartOfTable |         100 | 1.879 μs | 0.0457 μs | 0.1312 μs | 1.800 μs |     648 B |
     DeleteFromMiddleOfTable |         100 | 1.858 μs | 0.0406 μs | 0.0620 μs | 1.900 μs |     688 B |
        DeleteFromEndOfTable |         100 | 1.977 μs | 0.0433 μs | 0.1111 μs | 1.900 μs |     688 B |
  GetSubstringAtStartOfTable |         100 | 1.809 μs | 0.0396 μs | 0.1009 μs | 1.800 μs |     528 B |
 GetSubstringAtMiddleOfTable |         100 | 2.058 μs | 0.0449 μs | 0.0584 μs | 2.100 μs |     600 B |
    GetSubstringAtEndOfTable |         100 | 2.106 μs | 0.0456 μs | 0.1194 μs | 2.100 μs |     592 B |
      **InsertIntoTableAtStart** |        **1000** | **3.619 μs** | **0.1103 μs** | **0.2982 μs** | **3.500 μs** |    **2744 B** |
     InsertIntoTableAtMiddle |        1000 | 3.915 μs | 0.1377 μs | 0.3652 μs | 3.800 μs |    2840 B |
        InsertIntoTableAtEnd |        1000 | 3.729 μs | 0.1475 μs | 0.3988 μs | 3.600 μs |    2744 B |
      DeleteFromStartOfTable |        1000 | 2.207 μs | 0.0922 μs | 0.2492 μs | 2.200 μs |     648 B |
     DeleteFromMiddleOfTable |        1000 | 2.077 μs | 0.0518 μs | 0.1391 μs | 2.000 μs |     688 B |
        DeleteFromEndOfTable |        1000 | 2.270 μs | 0.0767 μs | 0.2113 μs | 2.200 μs |     688 B |
  GetSubstringAtStartOfTable |        1000 | 2.133 μs | 0.0848 μs | 0.2321 μs | 2.100 μs |     528 B |
 GetSubstringAtMiddleOfTable |        1000 | 2.567 μs | 0.1136 μs | 0.3147 μs | 2.500 μs |     600 B |
    GetSubstringAtEndOfTable |        1000 | 2.431 μs | 0.1200 μs | 0.3306 μs | 2.300 μs |     592 B |
      **InsertIntoTableAtStart** |       **10000** | **3.792 μs** | **0.2368 μs** | **0.6678 μs** | **3.600 μs** |    **2856 B** |
     InsertIntoTableAtMiddle |       10000 | 4.380 μs | 0.2827 μs | 0.7973 μs | 4.250 μs |    2952 B |
        InsertIntoTableAtEnd |       10000 | 3.974 μs | 0.2875 μs | 0.8155 μs | 3.700 μs |    2856 B |
      DeleteFromStartOfTable |       10000 | 4.229 μs | 0.4177 μs | 1.1848 μs | 3.600 μs |     648 B |
     DeleteFromMiddleOfTable |       10000 | 4.234 μs | 0.4199 μs | 1.2183 μs | 3.600 μs |     688 B |
        DeleteFromEndOfTable |       10000 | 4.059 μs | 0.3776 μs | 1.0956 μs | 3.400 μs |     688 B |
  GetSubstringAtStartOfTable |       10000 | 3.191 μs | 0.2318 μs | 0.6424 μs | 3.200 μs |     528 B |
 GetSubstringAtMiddleOfTable |       10000 | 4.780 μs | 0.4638 μs | 1.3231 μs | 4.650 μs |     600 B |
    GetSubstringAtEndOfTable |       10000 | 6.477 μs | 0.7928 μs | 2.2099 μs | 6.300 μs |     592 B |
