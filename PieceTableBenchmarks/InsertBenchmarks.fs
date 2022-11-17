namespace PieceTableBenchmarks

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Jobs
open PieceTable
open PieceTable.TextTable

module InsertData = 
    let empty = TextTable.create ""
    let mutable table = TextTable.create ""
    let mutable tableLength = 0

[<MemoryDiagnoser; HtmlExporter>]
type Insert() =
    [<Params(100, 1000)>]
    member val size = 0 with get, set

    [<IterationSetup>]
    member this.createWithPieces() =
        InsertData.table <- TextTable.create ""
        InsertData.tableLength <- 0
        for i in [0..this.size] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table <- InsertData.table.Insert(0, "hello")

    [<Benchmark>]
    member this.InsertNearEndOfTable() =
        InsertData.table <- InsertData.table.Insert(InsertData.tableLength - 3, "A")

    [<Benchmark>]
    member this.InsertNearStartOfTable() = 
        InsertData.table <- InsertData.table.Insert(3, "A")

    [<Benchmark>]
    member this.InsertAtMiddleOfTable() =
        let middle = InsertData.tableLength / 2
        InsertData.table <- InsertData.table.Insert(middle, "A")

module Main = 
    [<EntryPoint>]
    let Main _ =
        BenchmarkRunner.Run<Insert>() |> ignore
        0