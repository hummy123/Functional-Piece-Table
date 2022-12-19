namespace PieceTable

open FSharpx.Collections
open System.Globalization
open StringInfoExtensions
open Types

(* Inspired by Chris Okasaki's Red Black Tree design and F# implementation at 
 * https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Binary_Search_Trees *)
 
module Buffer =
    (* The maximum number of characters permitted in a buffer, before we create a new one. *)
    [<Literal>]
    let private MaxBufferLength = 1024

    /// An empty buffer.
    let empty = {HashMap = PersistentHashMap.empty; Length = 0}
                
    let private insertLongString (str: StringInfo) maxKeyInTree map =
        let rec loop curKey loopNum start newMap =
            if start >= str.Length 
            then newMap
            else
                let subStr = str[start..(loopNum * MaxBufferLength) - 1] |> StringInfo
                let nextKey = curKey + 1
                let nextLoopNum = loopNum + 1
                let nextTree = PersistentHashMap.add nextKey subStr newMap
                let nextStart = start + MaxBufferLength
                loop nextKey nextLoopNum nextStart nextTree
        loop maxKeyInTree 1 0 map

    let private add str (map: PersistentHashMap<int, StringInfo>) =
        let mapLength = map.Count - 1
        let strInfo = StringInfo str
        let curVal = map.Item mapLength
        if curVal.Length + strInfo.Length <= MaxBufferLength
        then 
            PersistentHashMap.remove mapLength map
            |> PersistentHashMap.add mapLength (curVal.Concat str)
        elif curVal.Length = MaxBufferLength
        then insertLongString strInfo mapLength map
        elif curVal.Length < MaxBufferLength && curVal.Length + strInfo.Length > MaxBufferLength
        then 
            let remainingBufferLength = MaxBufferLength - curVal.Length
            let fitString = strInfo[0..remainingBufferLength - 1]
            PersistentHashMap.remove mapLength map
            |> PersistentHashMap.add mapLength (curVal.Concat fitString)
            |> insertLongString (StringInfo strInfo[fitString.Length..]) mapLength
        else failwith "unexpected Buffer.tryAppend case"

    /// Append a string to the buffer.
    let append str buffer =
        if buffer.HashMap.Count = 0
        then 
            let strInfo = StringInfo str
            let tree = insertLongString strInfo -1 empty.HashMap
            { HashMap = tree; Length = strInfo.Length }
        else
            let tree = add str buffer.HashMap
            { HashMap = tree; Length = buffer.Length + str.Length }

    /// Create a buffer with a string.
    let createWithString str = append str empty

    /// Find the string associated with a particular key.
    let rec private nodeSubstring key startPos endPos (map: PersistentHashMap<int, StringInfo>) = 
        (map.Item key)[startPos..endPos]

    /// Gets text in a buffer at a specific span.
    let substring (span: SpanType) buffer = 
        (* Calculate the buffer key we need to traverse to find the span's data. *)
        let startKey = span.Start / MaxBufferLength
        let startBufferIndex = 
            if startKey = 0 
            then span.Start
            else span.Start % MaxBufferLength

        let endPos = Span.stop span
        let endKey = endPos / MaxBufferLength
        let endBufferIndex = 
            if endKey = 0 
            then endPos - 1
            else (endPos % MaxBufferLength) - 1
        
        if startKey = endKey 
        then nodeSubstring startKey startBufferIndex endBufferIndex buffer.HashMap
        elif startKey = endKey - 1
        then
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength buffer.HashMap
            let endStr = nodeSubstring endKey 0 endBufferIndex buffer.HashMap
            startStr + endStr
        else 
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength buffer.HashMap
            let endStr = nodeSubstring endKey 0 endBufferIndex buffer.HashMap
            
            let midStrRange = [|startKey + 1..endKey - 1|]
            let midStr = 
                Array.fold (fun acc key -> acc + (nodeSubstring key 0 MaxBufferLength buffer.HashMap)) "" midStrRange
            startStr + midStr + endStr

    /// OOP API for substring method for testing, as SpanTypeis internal to this assembly.
    type BufferType with
        member this.Substring(index, length) = 
            let span = Span.createWithLength index length
            substring span this