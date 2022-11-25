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
        printfn "case 1:\n"
        let table = initialTable.Insert(text.Length, insText)
        let table = table.Delete(1, 1)
        printfn "%A" <| table.Text()
        printfn "\ncase 2:\n"
        let table = initialTable.Insert(0, insText)
        let table = table.Delete(0,10)
        let expectedStr = (insText + text).Substring(10)
        printfn "expected: \n%s\ngot: \n%s" expectedStr (table.Text())
        0
