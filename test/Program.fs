open PieceTable
open PieceTable.TextTable
open System.Collections

let rnd = System.Random()

module Program =
    [<EntryPoint>]
    let main _ = 
        let mutable runningStr = ""

        (* Act, add strings to buffer repeatedly. *)
        let mutable buffer = Buffer.empty
        for i in [0..5] do
            let str = String.replicate 40000 "a"
            runningStr <- runningStr + str
            buffer <- Buffer.append str buffer

        0
