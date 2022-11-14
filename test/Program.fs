open PieceTable
open PieceTable.TextTable

[<Literal>]
let text = "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC)."

[<Literal>]
let insText = "TEST!"

module Program =
    [<EntryPoint>]
    let main _ = 
        let table = TextTable.create text
        let table = table.Insert(3, insText)
        printfn "Text:"
        printfn "%A" (table.Text())
        printfn "Pieces: "
        for i in table.Pieces do
            printfn "%A" i
        0
