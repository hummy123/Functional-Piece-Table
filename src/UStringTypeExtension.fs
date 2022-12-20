namespace PieceTable

open NStack

module UStringTypeExtension =
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
    type ustring with
        member this.GetSlice(start, finish) =
            let start, finish = ComputeSlice 0 start finish this.Length
            let len = finish - start + 1
            if len <= 0 then ustring.Empty
            else this.RuneSubstring(start, len)
