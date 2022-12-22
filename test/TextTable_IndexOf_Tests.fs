module TextTableIndexOfTests

open System
open System.Collections
open Xunit
open PieceTable
open FsCheck

let stringGen min max =
    let chars = Gen.sample min max Arb.generate<char>
    let chars = Array.ofList chars
    string(chars)

let substringGen (str: string) =
    if str.Length % 2 = 0 then
        let halfLength = str.Length / 2
        let min = Gen.choose(0, halfLength - 1) |> Gen.sample 0 1 |> Seq.exactlyOne
        let max = Gen.choose(halfLength, str.Length - 1) |> Gen.sample 0 1 |> Seq.exactlyOne
        str[min..max]
    else
        stringGen 1 20    

[<Fact>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in one buffer`` () =
    for i in [0..100] do
        let str = stringGen 1 (Buffer.MaxBufferLength - 1)
        let substr = substringGen str
        let expected = str.IndexOf(substr)
        let actual = TextTable.indexOf substr <| TextTable.create str
        Assert.Equal(expected, actual)

[<Fact>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in two buffers`` () =
    for i in [0..100] do
        let str = stringGen Buffer.MaxBufferLength ((Buffer.MaxBufferLength * 2) - 1)
        let substr = substringGen str
        let expected = str.IndexOf(substr)
        let actual = TextTable.indexOf substr <| TextTable.create str
        Assert.Equal(expected, actual)

[<Fact>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in three buffers`` () =
    for i in [0..100] do
        let str = stringGen (Buffer.MaxBufferLength * 2) ((Buffer.MaxBufferLength * 3) - 1)
        let substr = substringGen str
        let expected = str.IndexOf(substr)
        let actual = TextTable.indexOf substr <| TextTable.create str
        Assert.Equal(expected, actual)

[<Fact>]
let ``TextTable.indexOf = string.IndexOf fwhen substring crosses buffer nodes`` () =
    let baseString = String.replicate (Buffer.MaxBufferLength - 5) "a"
    let substrToSearchFor = String.replicate 10 "b"
    let strToSearchIn = baseString + substrToSearchFor
    let expected = strToSearchIn.IndexOf(substrToSearchFor)
    let actual = TextTable.indexOf substrToSearchFor <| TextTable.create strToSearchIn
    Assert.Equal(expected, actual)