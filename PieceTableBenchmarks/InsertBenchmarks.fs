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
    let mutable table_1_000_000 = TextTable.create ""
    let mutable tableLength = 0

    let loopOverTable size (table: byref<Types.TextTableType>) =
        if tableLength >= (size * 5) - 1 then
            ()
        else
            tableLength <- 0
            for i in [0..size] do
                tableLength <- tableLength + 5
                table <- table.Insert(0, "hello")

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type Insert() =
    [<Params(100, 1_000, 10_000, 100_000, 1_000_000)>]
    member val size = 0 with get, set

    [<IterationSetup>]
    member this.createWithPieces() =
        let table = 
            if this.size = 100 then &InsertData.table_100
            elif this.size = 1000 then &InsertData.table_1000
            elif this.size = 10_000 then &InsertData.table_10_000
            elif this.size = 100_000 then &InsertData.table_100_000
            else &InsertData.table_1_000_000
        InsertData.loopOverTable this.size &table

    [<Benchmark>]
    member this.InsertNearStartOfTable() = 
        InsertData.table_100 <- InsertData.table_100.Insert(3, "A")

    [<Benchmark>]
    member this.InsertAtMiddleOfTable() =
        let middle = InsertData.tableLength / 2
        InsertData.table_100 <- InsertData.table_100.Insert(middle, "A")

    [<Benchmark>]
    member this.InsertNearEndOfTable() =
        InsertData.table_100 <- InsertData.table_100.Insert(InsertData.tableLength - 3, "A")

module Main = 
    [<EntryPoint>]
    let Main _ =
        BenchmarkRunner.Run<Insert>() |> ignore
        0