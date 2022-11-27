namespace PieceTable

open Types

module TextTable =
    /// Create a TextTableType given a string,
    let create (str: string) =
        let piece = Piece.create true 0 str.Length
        { OriginalBuffer = str
          AddBuffer = ""
          Pieces =
            if str = "" then
                ListZipper.empty
            else
                ListZipper.createWithPiece piece
          Tree = 
            if str = "" then
                Tree.insert 0 piece Tree.empty
            else
                Tree.make E piece E
        }

    let text table = 
        Tree.print table table.Tree
        ListZipper.text table

    /// Returns a new table with the string inserted.
    let insert index (str: string) (table: TextTableType) =
        let addBuffer = table.AddBuffer + str
        let piece = Piece.create false (addBuffer.Length - str.Length) str.Length
        let pieces = ListZipper.insert index piece table.Pieces
        let tree = Tree.insert index piece table.Tree

        { table with
            Pieces = pieces
            AddBuffer = addBuffer 
            Tree = tree
        }

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this

        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            ListZipper.delete span this

        member this.TextSlice(startIndex, length) =
            let span = Span.createWithLength startIndex length
            ListZipper.textSlice span this