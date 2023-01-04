namespace PieceTable

open AaTree
open Types
open Piece

module PieceTree =
    let empty = E
    let create piece = Piece { Piece = piece; Index = Index.empty; Lines = Lines.empty }

    let inline private pieceLength node = 
        match node with
        | E -> 0
        | T(_, _, Piece n, _) -> n.Piece.Length
        | _ -> failwith "tried to call pieceLength on a non-piece node"

    let rec private insMin piece node =
        match node with
        | E -> piece |> create |> leaf
        | T(h, l, Piece v, r) ->
            let v = { v with Index = Index.plusLeft piece.Length v.Index } |> Piece
            T(h, insMin piece l, v, r) |> skew |> AaTree.split
        | _ -> failwith "tried to call insMin on a non-piece node"

    let rec private insMax piece node =
        match node with
        | E -> piece |> create |> leaf
        | T(h, l, Piece v, r) ->
            let v = { v with Index = Index.plusRight piece.Length v.Index } |> Piece
            T(h, l, v, insMin piece r) |> skew |> AaTree.split
        | _ -> failwith "tried to call insMax on a non-piece node"

    /// Returns the text contained in the PieceTree.
    let text table = 
        let folder = (fun (acc: string) n ->
            match n with
            | Piece n ->
                let text = (Buffer.substring n.Piece.Start n.Piece.Length table.Buffer)
                acc + text
            | _ -> failwith "tried to call PieceTree.text on a non-piece node"
        )
        fold folder "" table.Pieces

    let insert insIndex piece tree =
        let rec ins curIndex node =
            match node with
            | E -> piece |> create |> leaf
            | T(h, l, Piece v, r) ->
                let nodeEndIndex = curIndex + v.Piece.Length                
                if insIndex > nodeEndIndex then 
                    let newIndex = Index.plusRight piece.Length v.Index
                    let newNode = {v with Index = newIndex} |> Piece
                    let nextIndex = nodeEndIndex + sizeLeft r
                    T(h, l, newNode, ins nextIndex r) |> skew |> AaTree.split
                elif insIndex < curIndex then
                    let newIndex = Index.plusLeft piece.Length v.Index
                    let newNode = {v with Index = newIndex} |> Piece
                    let nextIndex = curIndex - pieceLength l - sizeRight l
                    T(h, ins nextIndex l, newNode, r) |> skew |> AaTree.split
                elif curIndex = insIndex then
                    let newIndex = Index.plusLeft piece.Length v.Index
                    let newNode = {v with Index = newIndex} |> Piece
                    T(h, insMax piece l, newNode, r) |> skew |> AaTree.split
                elif insIndex = nodeEndIndex then
                    let newPiece = Piece.merge v.Piece piece
                    let newNode = {v with Piece = newPiece} |> Piece
                    T(h, l, newNode, r)
                else
                    // We are in range.
                    let (p1, p3) = Piece.split v.Piece (insIndex - curIndex)
                    let newLeft = insMax p1 l
                    let newRight = insMin p3 r
                    let newIndex = Index.create (size newLeft) (size newRight)
                    let newNode = { v with Index = newIndex; Piece = piece } |> Piece
                    T(h, newLeft, newNode, newRight) |> skew |> AaTree.split
            | _ -> failwith "tried to call PieceTree.insert on a non-PieceTree node"

        ins (sizeLeft tree) tree

    (* Repeated if-statements used in both delete and substring. *)
    let inline private inRange start curIndex finish nodeEndIndex =
        start <= curIndex && finish >= nodeEndIndex

    let inline private startIsInRange start curIndex finish nodeEndIndex =
        start <= curIndex && finish < nodeEndIndex && curIndex < finish

    let inline private endIsInRange start curIndex finish nodeEndIndex =
        start > curIndex && finish >= nodeEndIndex && start <= nodeEndIndex

    let inline private middleIsInRange start curIndex finish nodeEndIndex =
        start >= curIndex && finish <= nodeEndIndex

    let substring (start: int) (length: int) table =
        let finish = start + length
        let rec sub curIndex node acc =
            match node with
            | E -> acc
            | T(h, l, Piece v, r) ->
                let left =
                    if start < curIndex
                    then sub (curIndex - pieceLength l - sizeRight l) l acc
                    else acc

                let nodeEndIndex = curIndex + v.Piece.Length
                let middle = 
                    if inRange start curIndex finish nodeEndIndex then
                        left + Piece.text v.Piece table
                    elif startIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textAtStart curIndex finish v.Piece table
                    elif endIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textAtEnd curIndex start v.Piece table
                    elif middleIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textInRange curIndex start finish v.Piece table
                    else
                        left

                if finish > nodeEndIndex
                then sub (nodeEndIndex + sizeLeft r) r middle
                else middle
            | _ -> failwith "tried to call PieceTree.substring on a non-PieceTree node"

        sub (sizeLeft table.Pieces) table.Pieces ""

    let delete (start: int) (length: int) (tree: PieceTree): PieceTree =
        let finish: int = start + length
        let rec del (curIndex: int) (node: PieceTree) =
            match node with
            | E as empty -> empty
            | T(h, l, Piece v, r) as node ->
                let left =
                    if start < curIndex
                    then del (curIndex - pieceLength l - sizeRight l) l
                    else l
                let nodeEndIndex: int = curIndex + v.Piece.Length
                let right =
                    if finish > nodeEndIndex
                    then del (nodeEndIndex + sizeLeft r) r
                    else r
                
                if inRange start curIndex finish nodeEndIndex then
                    if left = E
                    then right
                    else 
                        let (newLeft, Piece newVal) = splitMax left
                        let idx = Index.create (size newLeft) (size right)
                        let newNode = {newVal with Index = idx } |> Piece
                        T(h, newLeft, newNode, right) |> adjust
                elif startIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtStart curIndex finish v.Piece
                    let idx = Index.create (size left) (size right)
                    let newNode = {v with Index = idx; Piece = newPiece } |> Piece
                    T(h, left, newNode, right) |> skew |> AaTree.split
                elif endIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtEnd curIndex start v.Piece
                    let idx = Index.create (size left) (size right)
                    let newNode = {v with Index = idx; Piece = newPiece } |> Piece
                    T(h, left, newNode, right) |> adjust
                elif middleIsInRange start curIndex finish nodeEndIndex then
                    let (p1, p2) = Piece.deleteInRange curIndex start finish v.Piece
                    let newLeft = insMax p1 left
                    let idx = Index.create (size newLeft) (size right)
                    let newNode = {v with Index = idx } |> Piece
                    T(h, newLeft, newNode, right) |> skew |> AaTree.split
                else
                    let idx = Index.create (size left) (size right)
                    let newNode = {v with Index = idx } |> Piece
                    T(h, left, newNode, right) |> adjust
                
        del (sizeLeft tree) tree 
    