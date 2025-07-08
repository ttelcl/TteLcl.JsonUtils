// (c) 2023  ttelcl / ttelcl

open System
open System.IO

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

let private runNormalize o =
  let outName =
    match o.OutputFile with
    | Explicit(fnm) ->
      fnm
    | Inplace ->
      o.InputFile
    | Auto ->
      if o.InputFile.EndsWith(".json") then
        o.InputFile.Substring(0, o.InputFile.Length-5) + ".normalized.json"
      elif o.InputFile.EndsWith(".jsonc") then
        // also convert JSONC -> JSON
        o.InputFile.Substring(0, o.InputFile.Length-6) + ".normalized.json"
      else
        failwith $"Expecting input file name to end with '.json'"
  let jsonInput = File.ReadAllText(o.InputFile)
  let settings = new JsonLoadSettings()
  settings.CommentHandling <- CommentHandling.Ignore
  let jtokenIn = JToken.Parse(jsonInput, settings);
  let jTokenOut = jtokenIn |> JsonNormalizer.Normalize
  let jsonOut =
    jTokenOut.ToString(Formatting.Indented)
  cp $"Saving \fg{outName}\f0"
  do
    use w = outName |> startFile
    jsonOut |> w.WriteLine
  outName |> finishFile
  0

let run arglist =
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
    o |> runNormalize

[<EntryPoint>]
let main args =
  try
    args |> Array.toList |> run
  with
  | ex ->
    ex |> fancyExceptionPrint verbose
    resetColor ()
    1



