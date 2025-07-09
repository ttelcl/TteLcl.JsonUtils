/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TteLcl.JsonRewrite.ModelTracking.Exporting;

/// <summary>
/// Implements <see cref="JsonModelExporter"/> to export to a
/// verbose JSON file.
/// </summary>
public class JsonFileFullExporter: JsonModelExporter
{
  /// <summary>
  /// Create a new JsonFileFullExporter
  /// </summary>
  public JsonFileFullExporter()
  {
  }

  /// <inheritdoc/>
  public override void Export(
    string fileName, SlotTracker model)
  {
    var exportedModel = ExportTracker(model);
    var json = JsonConvert.SerializeObject(exportedModel, Formatting.Indented);
    File.WriteAllText(fileName, json);
  }

  private JToken ExportTracker(SlotTracker tracker)
  {
    // variant one: serialize trackers as arrays
    var array = new JArray();
    var values =
      from kvp in tracker.TypeSlotMap
      orderby kvp.Key
      select kvp.Value;
    foreach(var slotvalue in values)
    {
      var exportedSlot = ExportValue(slotvalue);
      array.Add(exportedSlot);
    }
    return array;
  }

  private JToken ExportValue(SlotValue value)
  {
    return value switch {
      SlotValueArray a => ExportArrayValue(a),
      SlotValueObject o => ExportObjectValue(o),
      SlotValueBoolean v => ExportBooleanValue(v),
      SlotValueNull v => ExportNullValue(v),
      SlotValueNumber v => ExportNumberValue(v),
      SlotValueString v => ExportStringValue(v),
      SlotValueVoid v => ExportVoidValue(v),
      _ => throw new NotSupportedException(
        $"Unexpected slot value type {value.GetType().Name}"),
    };
  }

  private JObject ExportArrayValue(SlotValueArray array)
  {
    var result = ExportValueCommon(array);
    result["#minlen"] = array.MinLength;
    result["#maxlen"] = array.MaxLength;
    var content = new JArray();
    result["#content"] = content;
    var values =
      from kvp in array.ChildMap
      orderby kvp.Key
      select kvp.Value;
    foreach(var child in values)
    {
      content.Add(ExportValue(child));
    }
    return result;
  }

  private JObject ExportObjectValue(SlotValueObject value)
  {
    var result = ExportValueCommon(value);
    var pairs =
      from kvp in value.PropertyMap
      orderby kvp.Key
      select kvp;
    foreach(var kvp in pairs)
    {
      result[kvp.Key] = ExportTracker(kvp.Value);
    }
    return result;
  }

  private JObject ExportBooleanValue(SlotValueBoolean value)
  {
    var result = ExportValueCommon(value);
    result["#false"] = value.FalseCount;
    result["#true"] = value.TrueCount;
    return result;
  }

  private JObject ExportNullValue(SlotValueNull value)
  {
    return ExportValueCommon(value);
  }

  private JObject ExportNumberValue(SlotValueNumber value)
  {
    var result = ExportValueCommon(value);
    result["#integer"] = value.IsInteger;
    if(value.IsInteger)
    {
      result["#min"] = (long)value.MinValue;
      result["#max"] = (long)value.MaxValue;
    }
    else
    {
      result["#min"] = value.MinValue;
      result["#max"] = value.MaxValue;
    }
    return result;
  }

  private JObject ExportStringValue(SlotValueString value)
  {
    var result = ExportValueCommon(value);
    result["#minlen"] = value.MinLength;
    result["#maxlen"] = value.MaxLength;
    if(value.TrackedValues is not null)
    {
      var tracker = value.TrackedValues;
      if(tracker.Count > 0)
      {
        result["#distinct"] = tracker.Count;
        if((tracker.Count * 3 < value.Count * 2 && tracker.Count<=10) || tracker.Count <= 3)
        {
          // No more distinct values than 2/3 of the total number of strings observed
          // and no more than 10 distinct values,
          // or only a tiny number of distinct values (in particular: 1)
          var keys = new JObject();
          result["#values"] = keys;
          var counts =
            from kvp in tracker.Map
            orderby kvp.Value descending, kvp.Key
            select kvp;
          foreach(var count in counts)
          {
            keys[count.Key] = count.Value;
          }
        }
      }
    }
    return result;
  }

  private JObject ExportVoidValue(SlotValueVoid value)
  {
    return ExportValueCommon(value);
  }

  private JObject ExportValueCommon(SlotValue value)
  {
    var o = new JObject {
      ["#type"] = value.TypeKey,
      ["#count"] = value.Count
    };
    return o;
  }

}
