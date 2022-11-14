namespace PieceTableBenchmarks

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Jobs
open PieceTable
open PieceTable.TextTable

module InsertData = 
    let empty = TextTable.create ""
    let mutable table = TextTable.create ""

[<MemoryDiagnoser; HtmlExporter>]
type Insert() =
    [<Params(100, 1000)>]
    member val size = 0 with get, set

    [<IterationSetup>]
    member this.createWithPieces() =
        InsertData.table <- TextTable.create ""
        for i in [0..this.size] do
            InsertData.table <- InsertData.table.Insert(InsertData.table.DocumentLength, "hello")

    [<Benchmark>]
    member this.InsertNearEndOfTable() =
        InsertData.table <- InsertData.table.Insert(InsertData.table.DocumentLength - 3, "A")

    [<Benchmark>]
    member this.InsertNearStartOfTable() = 
        InsertData.table <- InsertData.table.Insert(3, "A")

    [<Benchmark>]
    member this.InsertAtMiddleOfTable() =
        let middle = InsertData.table.DocumentLength / 2
        InsertData.table <- InsertData.table.Insert(middle, "A")

module Main = 
    [<EntryPoint>]
    let Main _ =
        BenchmarkRunner.Run<Insert>() |> ignore
        0