open PieceTable
open PieceTable.TextTable
open System.Collections

[<Literal>]
let text =
    "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC)."

[<Literal>]
let insText = "TEST!"

let initialTable = TextTable.create text

module Program =
    [<EntryPoint>]
    let main _ =  
        let baseString = String.replicate (Buffer.MaxBufferLength - 5) "a"
        let substrToSearchFor = String.replicate 10 "b"
        let strToSearchIn = baseString + substrToSearchFor
        let actual = TextTable.indexOf substrToSearchFor <| TextTable.create strToSearchIn
        printfn "%A" <| actual
        0