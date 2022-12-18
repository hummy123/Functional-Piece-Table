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
        let table = TextTable.create ""
        let baseText = "12345678"
        let testText = (String.replicate (128 * 3) baseText)[1..]

        let buffer = Buffer.createWithString testText
        let buffer = Buffer.append baseText buffer
        let bufferText = Buffer.text buffer
        printfn "%s" <| table.Text()
        0
