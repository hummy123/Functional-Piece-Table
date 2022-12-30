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
<<<<<<< HEAD

[<Fact>]
let ``Can get a substring over a split piece`` () =
    let table = TextTable.create "123456789"
    let table = table.Insert(0, "a")
    let table = table.Insert(2, "b")
    let substr = table.Substring(0, 3)
    Assert.Equal("a1b", substr)

[<Fact>]
let ``TextEdit1`` () =
    // Enter string correctly.
    let table = TextTable.create "124567893"
    let table = table.Insert(8, "98")
    let table = table.Insert(9, "ab")
    let expWholestr = "124567899ab83"
    let wholestr = table.Text()
    // Check we have entered string correctly.
    Assert.Equal(expWholestr, wholestr)

    // Get substring.
    let expsubstr9 = "124567899"
    let substr9 = table.Substring(0, 9)
    Assert.Equal(expsubstr9, substr9)

[<Fact>]
let ``TextEdit2`` () =
    // Enter string correctly.
    let table = TextTable.create "123456789"
    let table = table.Insert(8, "a")
    let table = table.Insert(9, "b")
    let table = table.Insert(7, "c")
    let table = table.Insert(8, "d")
    let table = table.Insert(6, "e")
    let table = table.Insert(7, "f")
    let expWholestr = "123456ef7cd8ab9"
    let wholestr = table.Text()
    // Check we have entered string correctly.
    Assert.Equal(expWholestr, wholestr)

    let expsubstr = "123456e"
    let substr = table.Substring(0, 7)
    Assert.Equal(expsubstr, substr)

    let expsubstr = "123456ef"
    let substr = table.Substring(0, 8)
    Assert.Equal(expsubstr, substr)

    let expsubstr = "123456ef7"
    let substr = table.Substring(0, 9)
    Assert.Equal(expsubstr, substr)
=======
>>>>>>> main
