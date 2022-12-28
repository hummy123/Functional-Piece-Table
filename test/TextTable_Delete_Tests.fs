module TextTableSubstringTests

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
let ``Can get a substring from the start of a table's OriginalBuffer`` () =
    let substring = initialTable.Substring(0, 2)
    Assert.Equal("Du", substring)

[<Fact>]
let ``Can get a substring from the whole of a table's AddBuffer`` () =
    let table = initialTable.Insert(5, insText)
    let substring = table.Substring(5, insText.Length)
    Assert.Equal("TEST!", substring)

[<Fact>]
let ``Can get a substring from around a table's AddBuffer`` () =
    let table = initialTable.Insert(5, insText)
    let substring = table.Substring(4, insText.Length + 2)
    Assert.Equal("nTEST!g", substring)

[<Fact>]
let ``Can get a substring from the end of a table's OriginalBuffer`` () = 
    let substring = initialTable.Substring(text.Length - 5, 5)
    Assert.Equal(text.Substring(text.Length - 5, 5), substring)

[<Fact>]
let ``Can get a substring from the end of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let substring = table.Substring(text.Length - 5, 5)
    Assert.Equal(text.Substring(text.Length - 5, 5), substring)

[<Fact>]
let ``Can get a substring from the middle of a table's OriginalBuffer`` () = 
    let substring = initialTable.Substring(1, 1)
    let expectedStr = "u"
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get a substring from the middle of a table's AddBuffer`` () =
    let table = initialTable.Insert(1, "abc")
    let substring = table.Substring(2,1)
    let expectedStr = "b"
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get a substring from start when zipper is at end`` () =
    let table = initialTable.Insert(text.Length, insText)
    let substring = table.Substring(0, 10)
    let expectedStr = text.Substring(0, 10)
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get a substring from the middle when zipper is at end`` () =
    let table = initialTable.Insert(text.Length, insText)
    let substring = table.Substring(10, 20)
    let expectedStr = text.Substring(10, 20)
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get a substring from end when zipper is at end`` () =
    let table = initialTable.Insert(text.Length, insText)
    let substring = table.Substring(text.Length, insText.Length)
    let expectedStr = insText
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get a substring from start when zipper is in middle`` () =
    let table = initialTable.Insert(text.Length/2, insText)
    let substring = table.Substring(0,5)
    let expectedStr = text.Substring(0, 5)
    Assert.Equal(expectedStr, substring)

[<Fact>]
let ``Can get substring of middle when zipper is in middle`` () =
    let table = initialTable.Insert(text.Length/2, insText)
    let substring = table.Substring(text.Length/2, insText.Length)
    let expectedStr = insText
    Assert.Equal(expectedStr, substring)
