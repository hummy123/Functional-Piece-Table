namespace PieceTable

open Types

module internal Index =
    /// Index information when we are at leaf.
    let empty = { Left = 0; Right = 0 }: IndexType

    let create left right: IndexType = { Left = left; Right = right }

    let inline plusLeft (length: int) index: IndexType =
        { index with Left = index.Left + length }

    let inline plusRight (length: int) index: IndexType =
        { index with Right = index.Right + length }

    let inline minusLeft (text: string) index: IndexType =
        { index with Left = index.Left - text.Length }

    let inline minusRight (text: string) index: IndexType =
        { index with Right = index.Right - text.Length }

    let inline setLeft newLeft index: IndexType =
        { index with Left = newLeft }

    let inline setRight newRight index: IndexType =
        { index with Right = newRight }

    let inline size (index: IndexType) = index.Left + index.Right

module Lines =
    let empty: LineType = { Left = 0; Right = 0 }