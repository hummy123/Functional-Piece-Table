module TextTableIndexOfTests

open System
open System.Collections
open Xunit
open PieceTable
open FsCheck
open FsCheck.Xunit

let stringGen min max =
    let chars = Gen.sample min max Arb.generate<char>
    let chars = Array.ofList chars
    string(chars)

let substringGen (str: string) =
    if str.Length % 2 = 0 then
        let rnd = Random()
        let start = rnd.Next(0, str.Length - 1)
        let finish = rnd.Next(start, str.Length - 1)
        str[start..finish]
    else
        "Returning a substring very unlikely to be found in the str parameter and expecting indexOf = -1."  

let indexOfGen nodeNum =
    let start = Buffer.MaxBufferLength * (nodeNum - 1)
    let finish = Buffer.MaxBufferLength * nodeNum
    let strLenGen = Gen.choose(start, finish) |> Arb.fromGen
    Prop.forAll strLenGen (fun strLen -> 
        let str = stringGen start strLen
        let substr = substringGen str
        let expected = str.IndexOf(substr)
        let actual = TextTable.indexOf substr <| TextTable.create str
        expected = actual)

[<Property>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in one buffer`` () =
    indexOfGen 1

[<Property>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in two buffers`` () =
    indexOfGen 2

[<Property>]
let ``TextTable.indexOf = string.IndexOf for strings fitting in three buffers`` () =
    indexOfGen 3

[<Fact>]
let ``TextTable.indexOf = string.IndexOf when substring crosses buffer nodes`` () =
    let baseString = String.replicate (Buffer.MaxBufferLength - 5) "a"
    let substrToSearchFor = String.replicate 10 "b"
    let strToSearchIn = baseString + substrToSearchFor
    let expected = strToSearchIn.IndexOf(substrToSearchFor)
    let actual = TextTable.indexOf substrToSearchFor <| TextTable.create strToSearchIn
    Assert.Equal(expected, actual)

let lastIndexOfGen nodeNum =
    let start = Buffer.MaxBufferLength * (nodeNum - 1)
    let finish = Buffer.MaxBufferLength * nodeNum
    let strLenGen = Gen.choose(start, finish) |> Arb.fromGen
    Prop.forAll strLenGen (fun strLen -> 
        let str = stringGen start strLen
        let substr = substringGen str
        let expected = str.LastIndexOf(substr)
        let actual = TextTable.lastIndexOf substr <| TextTable.create str
        expected = actual)

[<Property>]
let ``TextTable.lastIndexOf = string.LastIndexOf for strings fitting in one buffer`` () =
    lastIndexOfGen 1

[<Property>]
let ``TextTable.lastIndexOf = string.LastIndexOf for strings fitting in two buffers`` () =
    lastIndexOfGen 2

[<Property>]
let ``TextTable.lastIndexOf = string.LastIndexOf for strings fitting in three buffers`` () =
    lastIndexOfGen 3

[<Fact>]
let ``TextTable.lastIndexOf = string.LastIndexOf when substring crosses buffer nodes`` () =
    let baseString = String.replicate (Buffer.MaxBufferLength - 5) "a"
    let substrToSearchFor = String.replicate 10 "b"
    let strToSearchIn = baseString + substrToSearchFor
    let expected = strToSearchIn.LastIndexOf(substrToSearchFor)
    let actual = TextTable.lastIndexOf substrToSearchFor <| TextTable.create strToSearchIn
    Assert.Equal(expected, actual)
