open PieceTable
open PieceTable.TextTable
open System.Collections

module Program =
    [<EntryPoint>]
    let main _ = 
        let baseText = "12345"
        let text = String.replicate 13_108 baseText
        let buffer = Buffer.createWithString text
        let bufferText = Buffer.text buffer
        printfn "%A" text.Length
        0
