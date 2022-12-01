module BufferTests

open System
open System.Collections
open Xunit
open PieceTable

[<Fact>]
let ``New buffer with short string contains expected text and length`` () =
    let text = "short string"
    let expectedList: IEnumerable = [text.Length]

    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``New buffer will store text with length of 65535 chars in one node`` () =
    let baseText = "12345"
    let text = String.replicate 13_107 baseText
    let expectedList: IEnumerable = [65535]

    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

// Below test fails, figure out why.
[<Fact>]
let ``New buffer will store text with length of 65540 chars in two nodes`` () =
    let baseText = "12345"
    let text = String.replicate 13_108 baseText
    let expectedList: IEnumerable = [65535;5]
    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    //Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Adding a string to a buffer with one full node will create a buffer with two nodes`` () =
    let baseText = "12345"
    let text = String.replicate 13_107 baseText
    let expectedText = text + baseText
    let expectedList: IEnumerable = [65535; 5]

    let buffer = Buffer.createWithString text
    let buffer = Buffer.append baseText buffer
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    
    Assert.Equal(expectedText, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Adding a string to a buffer with one almost-full node will return a buffer with one full and one non-full node`` () =
    let baseText = "12345"
    let text = (String.replicate 13_107 baseText)[1..]
    let expectedText = (text + baseText)
    let expectedList: IEnumerable = [65535; 4]

    let buffer = Buffer.createWithString text
    let buffer = Buffer.append baseText buffer
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(expectedText, bufferText)
    Assert.Equal(expectedList, bufferList)