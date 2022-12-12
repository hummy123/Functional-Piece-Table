open PieceTable
open PieceTable.TextTable
open System.Collections

module Program =
    [<EntryPoint>]
    let main _ =  
        let mutable table = TextTable.create ""
        for i in [0..10] do
            table <- table.Insert(i, "j")
        PieceTree.print table.Pieces
        0
