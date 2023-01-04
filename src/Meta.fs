namespace PieceTable

open Types

module internal Meta =
    /// Index information when we are at leaf.
    let empty = { Left = 0; Right = 0 }

    let create left right = { Left = left; Right = right }

    let inline plusLeft (length: int) data =
        { data with Left = data.Left + length }

    let inline plusRight (length: int) data =
        { data with Right = data.Right + length }

    let inline minusLeft (text: string) data =
        { data with Left = data.Left - text.Length }

    let inline minusRight (text: string) data =
        { data with Right = data.Right - text.Length }

    let inline setLeft newLeft data =
        { data with Left = newLeft }

    let inline setRight newRight data =
        { data with Right = newRight }

    let inline size data = data.Left + data.Right
