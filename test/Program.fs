open PieceTable
open PieceTable.TextTable
open System.Collections

module Program =
    [<EntryPoint>]
    let main _ = 
        let mutable runningStr = ""
        let mutable buffer = Buffer.empty

        let rnd = System.Random()
        for i in [0..1000] do
            let str = String.replicate 8 (rnd.Next(1, 1_000_000_000).ToString())
            runningStr <- runningStr + str
            buffer <- Buffer.append str buffer
            
        let bufferText = Buffer.text buffer
        0
