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
let ``Initial table contains size of text`` () =
    Assert.Equal(initialTable.DocumentLength, text.Length)

[<Fact>]
let ``Initial table's text returns input text`` () =
    Assert.Equal(text, TextTable.text initialTable)

[<Fact>]
let ``Can insert into the start of an empty table`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, insText)
    Assert.Equal(insText.Length, table.DocumentLength)
    Assert.Equal(insText, table.Text())

[<Fact>]
let ``Can insert into the start of a table's OriginalBuffer`` () =
    let table = initialTable.Insert(0, insText)
    Assert.Equal(insText.Length + text.Length, table.DocumentLength)
    Assert.Equal(insText + text, table.Text())

[<Fact>]
let ``Can insert into the middle of a table's OriginalBuffer`` () =
    (* Test 1: Does the table have the expected length? *)
    let table = initialTable.Insert(3, insText)
    Assert.Equal(insText.Length + text.Length, table.DocumentLength)

    (* Test 2: Does the table return the expected string? *)
    let firstStr = text.Substring(0, 3)
    let thirdStr = text.Substring(3)
    let str = firstStr + insText + thirdStr
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the end of a table's OriginalBuffer`` () =
    (* Test 1: Does the table have the expected length? *)
    let table = initialTable.Insert(initialTable.DocumentLength, insText)
    Assert.Equal(insText.Length + text.Length, table.DocumentLength)

    (* Test 2: Does the table return the expected string? *)
    let str = text + insText
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the start of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(0, insText)
    Assert.Equal(text.Length + insText.Length, table.DocumentLength)
    Assert.Equal(insText + text, table.Text())

[<Fact>]
let ``Can insert into the middle of a table's AddBuffer`` () =
    (* Test 1: Does the table have the expected length? *)
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(3, insText)
    Assert.Equal(insText.Length + text.Length, table.DocumentLength)

    (* Test 2: Does the table return the expected string? *)
    let firstStr = text.Substring(0, 3)
    let thirdStr = text.Substring(3)
    let str = firstStr + insText + thirdStr
    Assert.Equal(str, table.Text())

[<Fact>]
let ``Can insert into the end of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Insert(text.Length, insText)
    Assert.Equal(text.Length + insText.Length, table.DocumentLength)
    Assert.Equal(text + insText, table.Text())

[<Fact>]
let ``Can delete from the start of a table's OriginalBuffer`` () =
    let table = initialTable.Delete(0, 2)
    Assert.Equal(text.Length - 2, table.DocumentLength)
    Assert.Equal(text.Substring(2), table.Text())

[<Fact>]
let ``Can delete from the start of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Delete(0, 2)
    Assert.Equal(text.Length - 2, table.DocumentLength)
    Assert.Equal(text.Substring(2), table.Text())

[<Fact>]
let ``Can delete from the end of a table's OriginalBuffer`` () = 
    let table = initialTable.Delete(initialTable.DocumentLength - 5, 5)
    Assert.Equal(text.Length - 5, table.DocumentLength)
    Assert.Equal(text.Substring(0, text.Length - 5), table.Text())

[<Fact>]
let ``Can delete from the end of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = table.Insert(0, text)
    let table = table.Delete(initialTable.DocumentLength - 5, 5)
    Assert.Equal(text.Length - 5, table.DocumentLength)
    Assert.Equal(text.Substring(0, text.Length - 5), table.Text())

[<Fact>]
let ``Can delete from the middle of a table's OriginalBuffer`` () = 
    let table = initialTable.Delete(1, 1)
    let expectedStr = text[0].ToString() + text.Substring(2)
    Assert.Equal(text.Length - 1, table.DocumentLength)
    Assert.Equal(expectedStr, table.Text())

[<Fact>]
let ``Can delete from the middle of a table's AddBuffer`` () =
    let table = TextTable.create ""
    let table = initialTable.Delete(1, 1)
    let expectedStr = text[0].ToString() + text.Substring(2)
    Assert.Equal(text.Length - 1, table.DocumentLength)
    Assert.Equal(expectedStr, table.Text())

let ``Can delete when deletion range incldes multiple pieces in a table.`` () =
    let table = initialTable.Insert(0, insText)
    let table = table.Delete(0,10)
    let expectedStr = (insText + text).Substring(10)
    Assert.Equal(text.Length + insText.Length - 10, table.DocumentLength)
    Assert.Equal(expectedStr, table.Text())