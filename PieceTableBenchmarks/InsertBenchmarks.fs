namespace PieceTableBenchmarks

open System
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Jobs
open PieceTable
open PieceTable.TextTable

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type CreateDocument() =
    [<Params(100, 1_000, 10_000)>]
    member val stringLength = 0 with get, set

    [<Benchmark>]
    member this.CreatePieceTableOfSize() = 
        let str = (String.replicate this.stringLength "a")
        TextTable.create str

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type InsertIntoDocument() =
    [<Params(100, 1000, 10_000)>]
    member val insertTimes = 0 with get, set

    member val table = TextTable.empty with get, set
    member val docLength = 0 with get, set

    [<IterationSetup>]
    member this.CreateDocument() =
        this.table <- TextTable.empty
        this.docLength <- 0
        for i in [0..this.insertTimes] do
            this.table <- this.table.Insert(0, "hello")
            this.docLength <- this.docLength + 5

    [<Benchmark>]
    member this.InsertIntoTableAtStart() = 
        this.table.Insert(0, "A")

    [<Benchmark>]
    member this.InsertIntoTableAtMiddle() =
        this.table.Insert(this.docLength / 2, "A")

    [<Benchmark>]
    member this.InsertIntoTableAtEnd() = 
        this.table.Insert(this.docLength, "A")

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type DeleteFromDocument() =
    [<Params(100, 1000, 10_000)>]
    member val insertTimes = 0 with get, set

    member val table = TextTable.empty with get, set
    member val docLength = 0 with get, set

    [<IterationSetup>]
    member this.CreateDocument() =
        this.table <- TextTable.empty
        this.docLength <- 0
        for i in [0..this.insertTimes] do
            this.table <- this.table.Insert(0, "hello")
            this.docLength <- this.docLength + 5

    [<Benchmark>]
    member this.DeleteFromStartOfTable() = 
        this.table.Delete(0, 10)

    [<Benchmark>]
    member this.DeleteFromMiddleOfTable() =
        this.table.Delete(this.docLength / 2, 10)

    [<Benchmark>]
    member this.DeleteFromEndOfTable() = 
        this.table.Delete(this.docLength - 10, 9)

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type GetSubstring() =
    [<Params(100, 1000, 10_000)>]
    member val insertTimes = 0 with get, set

    member val table = TextTable.empty with get, set
    member val docLength = 0 with get, set

    [<IterationSetup>]
    member this.CreateDocument() =
        this.table <- TextTable.empty
        this.docLength <- 0
        for i in [0..this.insertTimes] do
            this.table <- this.table.Insert(0, "hello")
            this.docLength <- this.docLength + 5

    [<Benchmark>]
    member this.GetSubstringAtStartOfTable() = 
        this.table.Substring(0, 10)

    [<Benchmark>]
    member this.GetSubstringAtMiddleOfTable() =
        this.table.Substring(this.docLength / 2, 10)

    [<Benchmark>]
    member this.GetSubstringAtEndOfTable() = 
        this.table.Substring(this.docLength - 10, 9)

[<MemoryDiagnoser; HtmlExporter; MarkdownExporter>]
type TableOperationsWhenSetupAlwaysInsertsAtEnd() =
    [<Params(100, 1000, 10_000)>]
    member val insertTimes = 0 with get, set

    member val table = TextTable.empty with get, set
    member val docLength = 0 with get, set

    [<IterationSetup>]
    member this.CreateDocument() =
        this.table <- TextTable.empty
        this.docLength <- 0
        for i in [0..this.insertTimes] do
            this.table <- this.table.Insert(this.docLength, "hello")
            this.docLength <- this.docLength + 5

    [<Benchmark>]
    member this.InsertIntoTableAtStart() = 
        this.table.Insert(0, "A")

    [<Benchmark>]
    member this.InsertIntoTableAtMiddle() =
        this.table.Insert(this.docLength / 2, "A")

    [<Benchmark>]
    member this.InsertIntoTableAtEnd() = 
        this.table.Insert(this.docLength, "A")

    [<Benchmark>]
    member this.DeleteFromStartOfTable() = 
        this.table.Delete(0, 10)

    [<Benchmark>]
    member this.DeleteFromMiddleOfTable() =
        this.table.Delete(this.docLength / 2, 10)

    [<Benchmark>]
    member this.DeleteFromEndOfTable() = 
        this.table.Delete(this.docLength - 10, 9)

    [<Benchmark>]
    member this.GetSubstringAtStartOfTable() = 
        this.table.Substring(0, 10)

    [<Benchmark>]
    member this.GetSubstringAtMiddleOfTable() =
        this.table.Substring(this.docLength / 2, 10)

    [<Benchmark>]
    member this.GetSubstringAtEndOfTable() = 
        this.table.Substring(this.docLength - 10, 9)

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
        BenchmarkRunner.Run<CreateDocument>() |> ignore
        BenchmarkRunner.Run<InsertIntoDocument>() |> ignore
        BenchmarkRunner.Run<DeleteFromDocument>() |> ignore
        BenchmarkRunner.Run<GetSubstring>() |> ignore
        BenchmarkRunner.Run<Consolidate>() |> ignore
        0