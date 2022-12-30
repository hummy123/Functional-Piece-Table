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
        // Enter string correctly.
        let table = TextTable.create "124567893"
        let table = table.Insert(8, "98")
        let table = table.Insert(9, "ab")
        let expWholestr = "124567899ab83"
        let wholestr = table.Text()

        // Get substring.
        let expsubstr9 = "124567899"
        let substr9 = table.Substring(0, 9)
        printfn "exp: %s" expsubstr9
        printfn "acc: %s" substr9
        0