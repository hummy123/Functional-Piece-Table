module Tests

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
let ``Can delete from the start of a table's OriginalBuffer`` () =
    let table = initialTable.Delete(0, 2)
    Assert.Equal(text.Substring(2), table.Text())

[<Fact>]
let ``Can delete from the start of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Delete(0, 2)
    Assert.Equal(text.Substring(2), table.Text())

[<Fact>]
let ``Can delete from the end of a table's OriginalBuffer`` () = 
    let table = initialTable.Delete(text.Length - 5, 5)
    Assert.Equal(text.Substring(0, text.Length - 5), table.Text())

[<Fact>]
let ``Can delete from the end of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Delete(text.Length - 5, 5)
    Assert.Equal(text.Substring(0, text.Length - 5), table.Text())

[<Fact>]
let ``Can delete from the middle of a table's OriginalBuffer`` () = 
    let table = initialTable.Delete(1, 1)
    let expectedStr = text[0].ToString() + text.Substring(2)
    Assert.Equal(expectedStr, table.Text())

[<Fact>]
let ``Can delete from the middle of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = initialTable.Delete(1, 1)
    let expectedStr = text[0].ToString() + text.Substring(2)
    Assert.Equal(expectedStr, table.Text())

[<Fact>]
let ``Can delete when deletion range incldes multiple pieces in a table.`` () =
    let table = initialTable.Insert(0, insText)
    let table = table.Delete(0,10)
    let expectedStr = (insText + text).Substring(10)
    Assert.Equal(expectedStr, table.Text())