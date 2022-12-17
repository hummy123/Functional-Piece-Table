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

type Insert() =
    [<Params(100, 1_000, 10_000)>]
    member val size = 0 with get, set

    [<IterationSetup>]
    member this.createWithPieces() =
        InsertData.table <- TextTable.create ""
        InsertData.tableLength <- 0
        for i in [0..this.size] do
            InsertData.tableLength <- InsertData.tableLength + 5
            InsertData.table <- InsertData.table.Insert(0, "hello")

    [<Benchmark>]
    member this.InsertNearStartOfTable() = 
        InsertData.table <- InsertData.table.Insert(3, "A")

    [<Benchmark>]
    member this.InsertAtMiddleOfTable() =
        let middle = InsertData.tableLength / 2
        InsertData.table <- InsertData.table.Insert(middle, "A")

    [<Benchmark>]
    member this.InsertNearEndOfTable() =
        InsertData.table <- InsertData.table.Insert(InsertData.tableLength - 3, "A")


type Consolidate() =
    [<Params(100, 1_000, 10_000)>]
    member val size = 0 with get, set
    member val docLength = 0 with get, set
    member val table = TextTable.empty with get, set

    [<IterationSetup>]
    member this.createWithPieces() =
        this.table <- TextTable.create ""
        this.docLength <- 0
        for i in [0..this.size] do
            this.docLength <- this.docLength + 5
            this.table <- this.table.Insert(0, "hello")

    [<Benchmark>]
    member this.ConsolidateTable() =
        TextTable.consolidate this.table

module Main = 
    [<EntryPoint>]
    let Main _ =
        //BenchmarkRunner.Run<Insert>() |> ignore
        BenchmarkRunner.Run<Consolidate>() |> ignore
        0