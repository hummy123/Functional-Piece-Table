open PieceTable
open PieceTable.TextTable

[<Literal>]
let text = "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC)."

[<Literal>]
let insText = "TEST!"

let initialTable = TextTable.create text

module Program =
    [<EntryPoint>]
    let main _ = 
        let table = initialTable.Insert(text.Length/2, insText)
        let table = table.Delete(text.Length, insText.Length)
        printfn "%A" <| table.Pieces.Index
        printfn "%A" <| table.Text()
        0
