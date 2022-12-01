module BufferAppendTests

open System
open System.Collections
open Xunit
open PieceTable

let rnd = Random()

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

[<Fact>]
let ``New buffer will store text with length of 65540 chars in two nodes`` () =
    let baseText = "12345"
    let text = String.replicate 13_108 baseText
    let expectedList: IEnumerable = [65535;5]
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

[<Fact>]
let ``Random test inserting small strings`` () =
    let mutable runningStr = ""
    let mutable buffer = Buffer.empty

    for i in [0..1000] do
        let str = rnd.Next(1, 1_000_000_000).ToString()
        runningStr <- runningStr + str
        buffer <- Buffer.append str buffer

        let bufferText = Buffer.text buffer
        let bufferList: IEnumerable = Buffer.lengthAsList buffer
        let expectedList: IEnumerable = [bufferText.Length]

        Assert.Equal(runningStr, bufferText)
        Assert.Equal(expectedList, bufferList)

[<Fact>]
let ``Random test inserting large strings`` () =
    let mutable runningStr = ""
    let mutable buffer = Buffer.empty

    for i in [0..1] do
        (* THERE IS AN ERROR WHEN WE REPLICATE BY 6654 OR ABOVE THAT GOES AWAY
         * WHEN WE DECREASE TO 6553 OR BELOW. FIND OUT WHY AND FIX. *)
        let str = String.replicate 6554 (rnd.Next(1_000_000_000, 1_000_000_000).ToString())
        runningStr <- runningStr + str
        buffer <- Buffer.append str buffer
        let bufferText = Buffer.text buffer

        Assert.Equal(runningStr.Length, bufferText.Length)
