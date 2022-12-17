module TextTableInsertTests

open System
open Xunit
open PieceTable
open PieceTable.TextTable

[<Literal>]
let text =
    "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC)."

[<Literal>]
let insText = "TEST!"

let initialTable = TextTable.create text

[<Fact>]
let ``Initial table's text returns input text`` () =
    Assert.Equal(text, TextTable.text initialTable)

[<Fact>]
let ``Can insert into the start of an empty table`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, insText)
    Assert.Equal(insText, table.Text())

[<Fact>]
let ``Can insert into the start of a table's OriginalBuffer`` () =
    let table = initialTable.Insert(0, insText)
    Assert.Equal(insText + text, table.Text())

[<Fact>]
let ``Can insert into the middle of a table's OriginalBuffer`` () =
    let table = initialTable.Insert(3, insText)
    let firstStr = text.Substring(0, 3)
    let thirdStr = text.Substring(3)
    let str = firstStr + insText + thirdStr
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the end of a table's OriginalBuffer`` () =
    let table = initialTable.Insert(text.Length, insText)
    let str = text + insText
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the start of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(0, insText)
    Assert.Equal(insText + text, table.Text())

[<Fact>]
let ``Can insert into the middle of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(3, insText)
    let firstStr = text.Substring(0, 3)
    let thirdStr = text.Substring(3)
    let str = firstStr + insText + thirdStr
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the end of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(text.Length, insText)
    Assert.Equal(text + insText, table.Text())

[<Fact>]
let ``Can continuously insert at start`` () =
    let mutable table = TextTable.create ""
    let mutable runningStr = ""
    for i in [0..10] do
        table <- table.Insert(0, "hello")
        runningStr <- runningStr + "hello"
        Assert.Equal(runningStr, table.Text())

[<Fact>]
let ``Can continuously insert at middle`` () =
    let mutable table = TextTable.create ""
    let mutable runningStr = ""
    for i in [0..10] do
        let halfLength = runningStr.Length / 2
        table <- table.Insert(halfLength, "hello")
        runningStr <- runningStr.Substring(0,halfLength) + "hello" + runningStr.Substring(halfLength)
    Assert.Equal(runningStr, table.Text())

[<Fact>]
let ``Can continuously insert at end`` () =
    let mutable table = TextTable.create ""
    let mutable runningStr = ""
    for i in [0..10] do
        table <- table.Insert(runningStr.Length, "hello")
        runningStr <- runningStr + "hello"
    Assert.Equal(runningStr, table.Text())