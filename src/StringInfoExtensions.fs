namespace PieceTable

open System.Globalization

/// Simple module to make some functions such as substring and length more concise
/// on the StringInfo class which handles Unicode characters correctly.
module StringInfoExtensions =
    /// ComputeSlice function authored by Microsoft and can be found at below link. 
    /// https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/prim-types.fs#L5891
    let inline private ComputeSlice bound start finish length =
        let low = 
            match start with
            | Some n when n >= bound -> n
            | _ -> bound
        let high = 
            match finish with 
            | Some m when m < bound + length -> m
            | _ -> bound + length - 1

        low, high

    type StringInfo with
        /// Returns LengthInTextElements.
        member inline this.Length = this.LengthInTextElements

        /// Gets a slice of a StringInfo using the same syntax one would can use 
        /// for a String, but respects Unicode code points.
        member this.GetSlice(start: int option, finish: int option) =
            let start, finish = ComputeSlice 0 start finish this.LengthInTextElements
            let len = finish - start + 1
            if len <= 0 then ""
            else this.SubstringByTextElements(start, len)

        /// Concatenates a String info with a string, returning a StringInfo.
        member this.Concat(other) =
            this.String + other |> StringInfo
