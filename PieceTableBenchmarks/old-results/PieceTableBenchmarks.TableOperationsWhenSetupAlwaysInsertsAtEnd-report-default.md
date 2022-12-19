
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-IWUGBN : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |     Mean |     Error |    StdDev |   Median | Allocated |
---------------------------- |------------ |---------:|----------:|----------:|---------:|----------:|
      **InsertIntoTableAtStart** |         **100** | **2.604 μs** | **0.2145 μs** | **0.6086 μs** | **2.400 μs** |    **1712 B** |
     InsertIntoTableAtMiddle |         100 | 2.234 μs | 0.0418 μs | 0.0815 μs | 2.200 μs |    1808 B |
        InsertIntoTableAtEnd |         100 | 2.198 μs | 0.0475 μs | 0.0869 μs | 2.200 μs |    1376 B |
      DeleteFromStartOfTable |         100 | 1.732 μs | 0.0381 μs | 0.0687 μs | 1.700 μs |     648 B |
     DeleteFromMiddleOfTable |         100 | 1.911 μs | 0.0401 μs | 0.0937 μs | 1.900 μs |     688 B |
        DeleteFromEndOfTable |         100 | 2.223 μs | 0.0718 μs | 0.2000 μs | 2.200 μs |     688 B |
  GetSubstringAtStartOfTable |         100 | 1.676 μs | 0.0533 μs | 0.1477 μs | 1.600 μs |     528 B |
 GetSubstringAtMiddleOfTable |         100 | 2.011 μs | 0.0552 μs | 0.1539 μs | 2.000 μs |     600 B |
    GetSubstringAtEndOfTable |         100 | 2.009 μs | 0.0498 μs | 0.1412 μs | 2.000 μs |     592 B |
      **InsertIntoTableAtStart** |        **1000** | **2.278 μs** | **0.0480 μs** | **0.1247 μs** | **2.200 μs** |    **2632 B** |
     InsertIntoTableAtMiddle |        1000 | 2.526 μs | 0.0825 μs | 0.2232 μs | 2.400 μs |    2728 B |
        InsertIntoTableAtEnd |        1000 | 2.468 μs | 0.0528 μs | 0.1373 μs | 2.400 μs |    2632 B |
      DeleteFromStartOfTable |        1000 | 2.379 μs | 0.1982 μs | 0.5590 μs | 2.200 μs |     648 B |
     DeleteFromMiddleOfTable |        1000 | 2.096 μs | 0.0770 μs | 0.2028 μs | 2.000 μs |     688 B |
        DeleteFromEndOfTable |        1000 | 2.424 μs | 0.0732 μs | 0.1941 μs | 2.400 μs |     688 B |
  GetSubstringAtStartOfTable |        1000 | 1.852 μs | 0.0720 μs | 0.1921 μs | 1.800 μs |     528 B |
 GetSubstringAtMiddleOfTable |        1000 | 2.233 μs | 0.0740 μs | 0.1949 μs | 2.200 μs |     600 B |
    GetSubstringAtEndOfTable |        1000 | 2.267 μs | 0.0641 μs | 0.1700 μs | 2.200 μs |     592 B |
      **InsertIntoTableAtStart** |       **10000** | **3.106 μs** | **0.2686 μs** | **0.7397 μs** | **2.850 μs** |    **2744 B** |
     InsertIntoTableAtMiddle |       10000 | 3.733 μs | 0.2139 μs | 0.5854 μs | 3.600 μs |    2840 B |
        InsertIntoTableAtEnd |       10000 | 3.288 μs | 0.3358 μs | 0.9249 μs | 3.000 μs |    2744 B |
      DeleteFromStartOfTable |       10000 | 4.442 μs | 0.4600 μs | 1.3273 μs | 4.600 μs |     648 B |
     DeleteFromMiddleOfTable |       10000 | 4.129 μs | 0.2572 μs | 0.7127 μs | 4.100 μs |     688 B |
        DeleteFromEndOfTable |       10000 | 5.061 μs | 0.4774 μs | 1.3849 μs | 5.300 μs |     688 B |
  GetSubstringAtStartOfTable |       10000 | 3.276 μs | 0.2793 μs | 0.7967 μs | 3.200 μs |     528 B |
 GetSubstringAtMiddleOfTable |       10000 | 4.483 μs | 0.2886 μs | 0.7996 μs | 4.400 μs |     600 B |
    GetSubstringAtEndOfTable |       10000 | 4.296 μs | 0.3925 μs | 1.1197 μs | 4.200 μs |     592 B |
