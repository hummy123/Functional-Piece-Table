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
        let table = initialTable.Insert(5, insText)
        let substring = table.Substring(4, insText.Length + 1)
        printfn "%s" <| substring
        printfn "%s" <| table.Text()
        0
