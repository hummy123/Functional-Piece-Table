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
        let mutable table = table.Insert(0, text)
        for i in [1..20] do
            table <- table.Insert(3, insText)
        printfn "%s" <| PieceTree.print table.Pieces
        0
