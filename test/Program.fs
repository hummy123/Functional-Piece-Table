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
        let mutable table = TextTable.create ""
        let mutable runningStr = ""
        for i in [0..10] do
            let halfLength = runningStr.Length / 2
            table <- table.Insert(halfLength, "hello")
            runningStr <- runningStr.Substring(0,halfLength) + "hello" + runningStr.Substring(halfLength)
        printfn "test"
        0
