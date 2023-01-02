namespace PieceTable

open Types

module internal Index =
    /// Index information when we are at leaf.
    let empty = { LeftSize = 0; RightSize = 0 }

    let create left right = { LeftSize = left; RightSize = right }

    let inline plusLeft (length: int) index =
        { index with LeftSize = index.LeftSize + length }

    let inline plusRight (length: int) index =
        { index with RightSize = index.RightSize + length }

    let inline minusLeft (text: string) index =
        { index with LeftSize = index.LeftSize - text.Length }

    let inline minusRight (text: string) index =
        { index with RightSize = index.RightSize - text.Length }

    let inline changeLeft newLeft index =
        { index with LeftSize = newLeft }

    let inline changeRight newRight index =
        { index with RightSize = newRight }

    let inline size index = index.LeftSize + index.RightSize