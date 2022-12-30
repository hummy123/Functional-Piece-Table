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
        let table = TextTable.create "123456789"
        let table = table.Insert(0, "a")
        let table = table.Insert(2, "b")
        let substr = table.Substring(0, 3)
        let expstr = "a1b"
        printfn "act: %s" substr
        printfn "exp: %s" expstr
        0