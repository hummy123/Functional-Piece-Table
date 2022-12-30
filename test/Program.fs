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
<<<<<<< HEAD
        // Enter string correctly.
        let table = TextTable.create "123456789"
        let table = table.Insert(8, "a")
        let table = table.Insert(9, "b")
        let table = table.Insert(7, "c")
        let table = table.Insert(8, "d")
        let table = table.Insert(6, "e")
        let table = table.Insert(7, "f")

        // Get substring.
        let substr9 = table.Substring(0, 9)
        printfn "acc: %s" substr9
=======
>>>>>>> main
        0