open PieceTable
open PieceTable.TextTable
open System.Collections
open System
[<Literal>]
let text =
    "During the development of the .NET Framework, the class libraries were originally written using a managed code compiler system called \"Simple Managed C\" (SMC)."

[<Literal>]
let insText = "TEST!"

let initialTable = TextTable.create text

module Program =
    [<EntryPoint>]
    let main _ =  
        let mutable runningStr = ""
        let mutable buffer = Buffer.empty

        let rnd = Random()
        for i in [0..1000] do
            let str = String.replicate 800 "a"
            runningStr <- runningStr + str
            buffer <- Buffer.append str buffer

            let bufferText = Buffer.text buffer
            let bufferList: IEnumerable = Buffer.lengthAsList buffer
            let a = "a"
            ()
        0
