namespace PieceTable

open System.Globalization

(*
 * In this module, we define a union of string (the normal case for text input)
 * amd StringInfo (handling grapheme clusters).
 * This is because StringInfo can be 10x slower for operations such as substring and 
 * concatenation and it is best only to use it if we need to handle grapheme clusters.
 *)

module UnicodeString =
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

    type UnicodeStringType = 
        | Plain of string
        | Unicode of StringInfo

        static member inline (+) (first, second) =
            match first, second with
            | Plain a, Plain b -> a + b |> Plain
            | Plain a, Unicode b -> a + b.String |> StringInfo |> Unicode
            | Unicode a, Plain b -> a.String + b |> StringInfo |> Unicode
            | Unicode a, Unicode b -> a.String + b.String |> StringInfo |> Unicode

        member inline this.Length =
            match this with
            | Plain s -> s.Length
            | Unicode s -> s.LengthInTextElements

        member inline this.GetSlice(start: int option, finish: int option) =
            let start, finish = ComputeSlice 0 start finish this.Length
            let len = finish - start + 1
            if len <= 0 then ""
            else
                match this with
                | Plain s -> s.Substring(start, len)
                | Unicode s -> s.SubstringByTextElements(start, len)

        member inline this.String =
            match this with
            | Plain s -> s
            | Unicode s -> s.String

        member inline this.GetEnumerator() =
            match this with
            | Plain s -> StringInfo.GetTextElementEnumerator(s)
            | Unicode s -> StringInfo.GetTextElementEnumerator(s.String)

    /// Creates a new UnicodeStringType instance.
    let inline create (str: string) =
        let strInfo = StringInfo str
        if strInfo.LengthInTextElements = str.Length
        then Plain str
        else Unicode strInfo

    /// Returns an array containing the string's line breaks.
    let rec lineBreaks (enumerator: TextElementEnumerator) (acc: ResizeArray<int>) =
        (* Internally, we use a mutable ResizeArray for add/append performance.
         * After processing, we convert it into an immutable array before return. *)
         if enumerator.MoveNext() then
            let chr = enumerator.GetTextElement()
            if chr.Contains("\r") || chr.Contains("\n") then
                acc.Add enumerator.ElementIndex
                lineBreaks enumerator acc
            else
                lineBreaks enumerator acc
         else
            acc.ToArray()
         
    type UnicodeStringType with
        member inline this.GetLineBreaks() = 
            lineBreaks (this.GetEnumerator()) (ResizeArray())