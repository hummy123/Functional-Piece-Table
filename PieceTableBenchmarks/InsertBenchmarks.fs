namespace PieceTableBenchmarks

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Jobs
open PieceTable
open PieceTable.TextTable

module InsertData = 
    let empty = TextTable.create ""
    let mutable table_100 = TextTable.create ""
    let mutable table_1000 = TextTable.create ""
    let mutable table_10_000 = TextTable.create ""
    let mutable table_100_000 = TextTable.create ""
    let mutable tableLength = 0

    let getTableBySize size =
        if size = 100 then &table_100
        elif size = 1000 then &table_1000
        elif size = 10_000 then &table_10_000
        else &table_100_000

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type Insert() =
    [<Params(100, 1_000, 10_000, 100_000)>]
    member val size = 0 with get, set

    [<Benchmark>]
    member this.InsertNearStartOfTable() = 
        let table = InsertData.getTableBySize this.size
        table.Insert(3, "A")

    [<Benchmark>]
    member this.InsertAtMiddleOfTable() =
        let table = InsertData.getTableBySize this.size
        let middle = InsertData.tableLength / 2
        table.Insert(middle, "A")

    [<Benchmark>]
    member this.InsertNearEndOfTable() =
        let table = InsertData.getTableBySize this.size
        table.Insert(InsertData.tableLength - 3, "A")

module Main = 
    [<EntryPoint>]
    let Main _ =
        for i in [0..100] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table_100 <- InsertData.table_100.Insert(0, "hello")

        InsertData.table_1000 <- InsertData.table_100
        for i in [101.1000] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table_1000 <- InsertData.table_1000.Insert(0, "hello")

        InsertData.table_10_000 <- InsertData.table_1000
        for i in [1001.10_000] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table_10_000 <- InsertData.table_10_000.Insert(0, "hello")

        InsertData.table_100_000 <- InsertData.table_10_000
        for i in [100_001..100_000] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table_100_000 <- InsertData.table_100_000.Insert(0, "hello")

        BenchmarkRunner.Run<Insert>() |> ignore
        0