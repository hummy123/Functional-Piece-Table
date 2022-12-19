
BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.1335/21H2)
AMD Ryzen 3 5300U with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2 DEBUG
  Job-JUFKHE : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

                      Method | insertTimes |     Mean |     Error |    StdDev |   Median | Allocated |
---------------------------- |------------ |---------:|----------:|----------:|---------:|----------:|
      **InsertIntoTableAtStart** |         **100** | **3.002 μs** | **0.1021 μs** | **0.2812 μs** | **2.900 μs** |    **1880 B** |
     InsertIntoTableAtMiddle |         100 | 2.882 μs | 0.0616 μs | 0.1062 μs | 2.900 μs |    1976 B |
        InsertIntoTableAtEnd |         100 | 2.813 μs | 0.0575 μs | 0.0860 μs | 2.800 μs |    1880 B |
      DeleteFromStartOfTable |         100 | 1.925 μs | 0.0358 μs | 0.0822 μs | 1.900 μs |     648 B |
     DeleteFromMiddleOfTable |         100 | 2.129 μs | 0.0950 μs | 0.2634 μs | 2.000 μs |     688 B |
        DeleteFromEndOfTable |         100 | 1.953 μs | 0.0591 μs | 0.1638 μs | 1.900 μs |     688 B |
  GetSubstringAtStartOfTable |         100 | 2.483 μs | 0.0660 μs | 0.1806 μs | 2.400 μs |     576 B |
 GetSubstringAtMiddleOfTable |         100 | 2.755 μs | 0.0590 μs | 0.1320 μs | 2.700 μs |     648 B |
    GetSubstringAtEndOfTable |         100 | 2.648 μs | 0.0581 μs | 0.1312 μs | 2.600 μs |     640 B |
      **InsertIntoTableAtStart** |        **1000** | **4.968 μs** | **0.3667 μs** | **0.9662 μs** | **4.700 μs** |    **3168 B** |
     InsertIntoTableAtMiddle |        1000 | 9.444 μs | 1.7483 μs | 5.0723 μs | 8.200 μs |    3264 B |
        InsertIntoTableAtEnd |        1000 | 8.641 μs | 1.5773 μs | 4.5761 μs | 6.700 μs |    3168 B |
      DeleteFromStartOfTable |        1000 | 3.326 μs | 0.4963 μs | 1.3999 μs | 2.700 μs |     648 B |
     DeleteFromMiddleOfTable |        1000 | 2.862 μs | 0.2483 μs | 0.6713 μs | 2.600 μs |     688 B |
        DeleteFromEndOfTable |        1000 | 2.735 μs | 0.1905 μs | 0.5183 μs | 2.500 μs |     688 B |
  GetSubstringAtStartOfTable |        1000 | 3.815 μs | 0.4954 μs | 1.3892 μs | 3.200 μs |     576 B |
 GetSubstringAtMiddleOfTable |        1000 | 7.411 μs | 1.4583 μs | 4.2539 μs | 5.450 μs |     720 B |
    GetSubstringAtEndOfTable |        1000 | 6.734 μs | 1.2886 μs | 3.7384 μs | 4.350 μs |     712 B |
      **InsertIntoTableAtStart** |       **10000** | **7.463 μs** | **0.8845 μs** | **2.4657 μs** | **6.300 μs** |    **3680 B** |
     InsertIntoTableAtMiddle |       10000 | 9.134 μs | 1.3510 μs | 3.8545 μs | 7.700 μs |    3776 B |
        InsertIntoTableAtEnd |       10000 | 8.184 μs | 1.2287 μs | 3.5055 μs | 6.400 μs |    3680 B |
      DeleteFromStartOfTable |       10000 | 4.658 μs | 0.6904 μs | 2.0031 μs | 4.100 μs |     648 B |
     DeleteFromMiddleOfTable |       10000 | 4.531 μs | 0.6066 μs | 1.7599 μs | 4.000 μs |     688 B |
        DeleteFromEndOfTable |       10000 | 4.317 μs | 0.5429 μs | 1.5665 μs | 3.900 μs |     688 B |
  GetSubstringAtStartOfTable |       10000 | 5.730 μs | 0.8907 μs | 2.5700 μs | 4.900 μs |     576 B |
 GetSubstringAtMiddleOfTable |       10000 | 6.189 μs | 0.7589 μs | 2.1404 μs | 6.000 μs |     720 B |
    GetSubstringAtEndOfTable |       10000 | 6.348 μs | 0.8405 μs | 2.4385 μs | 5.900 μs |     712 B |
