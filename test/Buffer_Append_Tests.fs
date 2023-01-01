module BufferAppendTests

open System
open System.Collections
open Xunit
open PieceTable

let rnd = Random()

/// Creates a list containing the expected value of Buffer.lengthAsList.
let stringLengthAsList (str: string) =
    let MaxBufferLen = Buffer.MaxBufferLength
    match str.Length <= MaxBufferLen with
    | true -> [str.Length]
    | false ->
        let loopTimes = str.Length / MaxBufferLen
        let remainChars = str.Length % MaxBufferLen
        let lengthAsList = List.map (fun _ -> MaxBufferLen) [0..loopTimes - 1]
        if remainChars = 0
        then lengthAsList
        else lengthAsList @ [remainChars]

[<Fact>]
let ``New buffer with short string contains expected text and length`` () =
    let text = "short string"
    let expectedList: IEnumerable = stringLengthAsList text

    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``New buffer will store text with length of 65535 chars in one node`` () =
    let baseText = "12345"
    let text = String.replicate 13_107 baseText
    let expectedList: IEnumerable = stringLengthAsList text

    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``New buffer will store text with length of 65540 chars in two nodes`` () =
    let baseText = "12345"
    let text = String.replicate 13_108 baseText
    let expectedList: IEnumerable = stringLengthAsList text
    let buffer = Buffer.createWithString text
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(text, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Adding a string to a buffer with one full node will create a buffer with two nodes`` () =
    let baseText = "12345"
    let text = String.replicate 13_107 baseText
    let expectedText = text + baseText
    let expectedList: IEnumerable = stringLengthAsList expectedText

    let buffer = Buffer.createWithString text
    let buffer = Buffer.appendString baseText buffer
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    
    Assert.Equal(expectedText, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Adding a string to a buffer with one almost-full node will return a buffer with one full and one non-full node`` () =
    let baseText = "12345"
    let text = (String.replicate 13_107 baseText)[1..]
    let expectedText = (text + baseText)
    let expectedList: IEnumerable = stringLengthAsList (text + baseText)

    let buffer = Buffer.createWithString text
    let buffer = Buffer.appendString baseText buffer
    let bufferText = Buffer.text buffer
    let bufferList: IEnumerable = Buffer.lengthAsList buffer
    Assert.Equal(expectedText, bufferText)
    Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Random test inserting small strings`` () =
    let mutable runningStr = ""
    let mutable buffer = Buffer.empty

    for i in [0..1000] do
        let str = rnd.Next(1, 1_000_000_000).ToString()
        runningStr <- runningStr + str
        buffer <- Buffer.appendString str buffer

        let bufferText = Buffer.text buffer
        let bufferList: IEnumerable = Buffer.lengthAsList buffer
        let expectedList: IEnumerable = stringLengthAsList bufferText

        Assert.Equal(runningStr, bufferText)
        Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Random test inserting large strings`` () =
    (* Arrange buffer's text content. *)
    let mutable runningStr = ""

    (* Act, add strings to buffer repeatedly. *)
    let mutable buffer = Buffer.empty
    for i in [0..500] do
        let str = String.replicate 100 (rnd.Next(1_000_000_000, 1_000_000_000).ToString())
        runningStr <- runningStr + str
        buffer <- Buffer.appendString str buffer

        let bufferText = Buffer.text buffer
        Assert.Equal(runningStr, bufferText)

    let expectedList : IEnumerable = stringLengthAsList runningStr

    let bufferList: IEnumerable = Buffer.lengthAsList buffer

    Assert.Equal(expectedList, bufferList)