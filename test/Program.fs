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
        let mutable table = TextTable.empty
        let mutable docLength = 0
        for i in [0..1] do
            table <- table.Insert(docLength, "hello")
            docLength <- docLength + 5

        0