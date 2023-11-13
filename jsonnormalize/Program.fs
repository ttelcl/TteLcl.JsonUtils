﻿// (c) 2023  ttelcl / ttelcl

open System

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open TteLcl.JsonRewrite.Miscellaneous

open CommonTools
open ExceptionTool
open Usage
open ColorPrint

type OutputName =
  | Auto
  | Inplace
  | Explicit of string

type private AppOptions = {
  InputFile: string
  OutputFile: OutputName
}

let rec run arglist =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | "--help" :: _
    | "-h" :: _ ->
      None
    | "-f" :: fnm :: rest ->
      rest |> parseMore {o with InputFile = fnm}
    | "-i" :: rest
    | "-inplace" :: rest
    | "--inplace" :: rest ->
      rest |> parseMore {o with OutputFile = OutputName.Inplace}
    | "-o" :: fnm :: rest ->
      rest |> parseMore {o with OutputFile = OutputName.Explicit(fnm)}
    | [] ->
      if o.InputFile = null then
        cp "\frNo input file name given\f0."
        None
      else
        Some(o)
    | x :: _ ->
      cp $"\frUnrecognized argument \fo{x}\f0."
      None
  let oo = arglist |> parseMore {
    InputFile = null
    OutputFile = OutputName.Auto
  }
  match oo with
  | None ->
    usage "all"
    1
  | Some(o) ->
    failwith "Not yet implemented"
    0

[<EntryPoint>]
let main args =
  try
    args |> Array.toList |> run
  with
  | ex ->
    ex |> fancyExceptionPrint verbose
    resetColor ()
    1



