module TextTableConsolidateTests

open System
open Xunit
open PieceTable
open PieceTable.TextTable

[<Fact>]
let ``Text before and after onsolidation is same when we keep inserting at start`` () =
    let mutable table = TextTable.empty
    for i in [0..10] do
        table <- TextTable.insert 0 "hello" table

    let beforeString = TextTable.text table
    table <- TextTable.consolidate table
    let afterString = TextTable.text table
    Assert.Equal(beforeString, afterString)

[<Fact>]
let ``Text before and after onsolidation is same when we keep inserting at middle`` () =
    let mutable table = TextTable.empty
    let mutable runningLength = 0
    for i in [0..10] do
        table <- TextTable.insert (runningLength / 2) "hello" table
        runningLength <- runningLength + 5

    let beforeString = TextTable.text table
    table <- TextTable.consolidate table
    let afterString = TextTable.text table
    Assert.Equal(beforeString, afterString)

[<Fact>]
let ``Text before and after onsolidation is same when we keep inserting at end`` () =
    let mutable table = TextTable.empty
    let mutable runningLength = 0
    for i in [0..10] do
        table <- TextTable.insert runningLength "hello" table
        runningLength <- runningLength + 5

    let beforeString = TextTable.text table
    table <- TextTable.consolidate table
    let afterString = TextTable.text table
    Assert.Equal(beforeString, afterString)

[<Fact>]
let ``Text before and after onsolidation is same when we keep inserting randomly`` () =
    let mutable table = TextTable.empty
    let mutable runningLength = 0
    let rnd = Random()
    for i in [0..10] do
        let insIndex = rnd.Next(0, runningLength)
        table <- TextTable.insert insIndex "hello" table
        runningLength <- runningLength + 5

    let beforeString = TextTable.text table
    table <- TextTable.consolidate table
    let afterString = TextTable.text table
    Assert.Equal(beforeString, afterString)