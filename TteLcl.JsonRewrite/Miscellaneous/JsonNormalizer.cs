/*
 * (c) 2023  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.JsonRewrite.Miscellaneous;

/// <summary>
/// Utility to create a new JSON model equivalent to a given one,
/// but with all object properties sorted in alphabetical order
/// </summary>
public static class JsonNormalizer
{
  /// <summary>
  /// Normalize a JSON fragment to one where all objects are replaced
  /// by objects where the properties are in alphabetical order
  /// </summary>
  public static JToken Normalize(JToken token)
  {
    return token.Type switch {
      JTokenType.Array => NormalizeArray((JArray)token),
      JTokenType.Object => NormalizeObject((JObject)token),
      _ => token.DeepClone(),
    };
  }

  private static JArray NormalizeArray(JArray array)
  {
    var result = new JArray();
    foreach(var item in array)
    {
      result.Add(Normalize(item));
    }
    return result;
  }

  private static JObject NormalizeObject(JObject obj)
  {
    var result = new JObject();
    var properties = obj.Properties().OrderBy(jp => jp.Name).ToList();
    foreach(var property in properties)
    {
      result.Add(property.Name, Normalize(property.Value));
    }
    return result;
  }
}
