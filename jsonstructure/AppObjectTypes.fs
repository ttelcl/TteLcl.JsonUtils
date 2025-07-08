module AppObjectTypes

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open TteLcl.JsonRewrite.ModelTracking

open CommonTools
open ColorPrint

type private Options = {
  InputFile: string
}

let private runObjectTypes o =
  let input =
    let json = File.ReadAllText(o.InputFile)
    let serializerSettings = new JsonSerializerSettings()
    serializerSettings.DateParseHandling <- DateParseHandling.None
    JsonConvert.DeserializeObject<JToken>(json, serializerSettings)
  let counters = new StringCounter(true)
  let trackerService = new TrackerService()
  let rec walkJson (node: JToken) =
    let key = trackerService.TokenKey(node)
    counters.Increment(key) |> ignore
    match node with
    | :? JArray as a ->
      for child in a do
        child |> walkJson
    | :? JObject as o ->
      for child in o.PropertyValues() do
        child |> walkJson
    | _ -> ()
  cp "Scanning input file ..."
  input |> walkJson
  let keys =
    counters.Keys
    |> Seq.sort
    |> Seq.toArray
  cp $"Found \fb{keys.Length}\f0 distinct types:"
  for key in keys do
    let count = counters[key]
    cp $"\fb{count,6} \fg{key}\f0."
  0

let run args =
  let rec parseMore o args =
    match args with
    | "-v" :: rest ->
      verbose <- true
      rest |> parseMore o
    | "-f" :: file :: rest ->
      rest |> parseMore {o with InputFile = file}
    | [] ->
      if o.InputFile |> String.IsNullOrEmpty then
        cp "\foNo input file specified\f0."
        None
      else
        o |> Some
    | x :: _ ->
      cp $"\foUnrecognized argument \fy{x}\f0."
      None
  let oo = args |> parseMore {
    InputFile = null
  }
  match oo with
  | None ->
    Usage.usage "objecttypes"
    1
  | Some(o) ->
    o |> runObjectTypes
  
