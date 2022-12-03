open PieceTable
open PieceTable.TextTable
open System.Collections

let rnd = System.Random()

module Program =
    [<EntryPoint>]
    let main _ = 
        let mutable runningStr = ""
        let mutable buffer = Buffer.empty
        buffer <- Buffer.append (String.replicate 65536 "a") buffer
        buffer <- Buffer.append "p" buffer
        
        for i in [0..1] do
            (* THERE IS AN ERROR WHEN WE REPLICATE BY 6654 OR ABOVE THAT GOES AWAY
             * WHEN WE DECREASE TO 6553 OR BELOW. FIND OUT WHY AND FIX. *)
            let str = String.replicate 6554 (rnd.Next(1_000_000_000, 1_000_000_000).ToString())
            runningStr <- runningStr + str
            
        0
