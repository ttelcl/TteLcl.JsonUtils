module AppAnalyze

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open TteLcl.JsonRewrite.ModelTracking
open TteLcl.JsonRewrite.ModelTracking.Exporting

open CommonTools
open ColorPrint

type private Options = {
  InputFile: string
  OutputFile: string
}

let private createExporter o =
  let outFileShortName = o.OutputFile |> Path.GetFileName
  let reverseOutputNameSegments =
    outFileShortName.ToLowerInvariant().Split('.')
    |> Seq.toList
    |> List.rev
  match reverseOutputNameSegments with
  | "json" :: "structure" :: _ ->
    new JsonFileFullExporter() |> Some
  | _ ->
    cp $"\frUnsupported output format in \fo{outFileShortName}\f0."
    cp "Supported output file formats are:"
    cp "  *\fg.structure.json\f0"
    cp "(other formats may be added in the future)"
    None

let private runAnalyze o =
  let exporterOption = o |> createExporter
  match exporterOption with
  | None ->
    1
  | Some(exporter) ->
    cp $"Loading \fg{o.InputFile}\f0."
    let input =
      let json = File.ReadAllText(o.InputFile)
      let serializerSettings = new JsonSerializerSettings()
      serializerSettings.DateParseHandling <- DateParseHandling.None
      JsonConvert.DeserializeObject<JToken>(json, serializerSettings)
    let trackerService = new TrackerService()
    let rootTracker = SlotTracker.CreateRootSlot(trackerService)
    rootTracker.TrackValue(input)
    cp $"Saving \fg{o.OutputFile}\f0."
    let tmpName = o.OutputFile + ".tmp"
    exporter.Export(tmpName, rootTracker)
    o.OutputFile |> finishFile
    0

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
  
