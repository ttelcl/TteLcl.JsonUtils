module AppAnalyze

open System
open System.IO

open CommonTools
open ColorPrint
open Usage

type private Options = {
  InputFile: string
  OutputFile: string
}

let private runAnalyze o =
  cp "\frNot yet implemented\f0."
  1

let run args =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | "-f" :: file :: rest ->
      rest |> parseMore {o with InputFile = file}
    | "-o" :: file :: rest ->
      rest |> parseMore {o with OutputFile = file}
    | [] ->
      if o.InputFile |> String.IsNullOrEmpty then
        cp "\foNo input file specified\f0."
        None
      else
        let o =
          if o.OutputFile |> String.IsNullOrEmpty then
            let outputFile =
              Path.ChangeExtension(o.InputFile |> Path.GetFileName, ".structure.json")
            {o with OutputFile = outputFile}
          else
            o
        o |> Some
    | x :: _ ->
      cp $"\foUnrecognized argument \fy{x}\f0."
      None
  let oo = args |> parseMore {
    InputFile = null
    OutputFile = null
  }
  match oo with
  | None ->
    Usage.usage "analyze"
    1
  | Some(o) ->
    o |> runAnalyze
  
