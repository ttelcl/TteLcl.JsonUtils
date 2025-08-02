/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TteLcl.JsonRewrite.ModelTracking;

/// <summary>
/// Defines unique keys for Json elements based on their type.
/// For most elements only the token type matters, but for Json objects
/// the field names are taken into account.
/// </summary>
public class TrackerService
{
  /// <summary>
  /// Create a new TrackerService
  /// </summary>
  public TrackerService()
  {
  }

  /// <summary>
  /// Get a key identifying the type of <see cref="JToken"/>.
  /// For objects this includes an alphabetically sorted list of 
  /// property names.
  /// </summary>
  /// <param name="token">
  /// The token to get the key name for. If null, "void" is returned.
  /// </param>
  /// <returns></returns>
  /// <exception cref="NotSupportedException"></exception>
  public string TokenKey(JToken? token)
  {
    return token switch {
      null => "void",
      JObject o => ObjectToken(o),
      JValue v => ValueToken(v),
      JArray => "array",
      _ => throw new NotSupportedException(
        $"Unsupported JSON token kind {token.GetType().Name} ({token.Type})"),
    };
  }

  /// <summary>
  /// Get the set of property names in the object, ordered by property name
  /// </summary>
  public HashSet<string> ObjectFieldSet(JObject o)
  {
    var propNames =
      from prop in o.Properties()
      select prop.Name into n
      orderby n
      select n;
    return propNames.ToHashSet();
  }

  private static char[] __needQuoteCharacters = " ()[]{}".ToCharArray();

  /// <summary>
  /// Build an object key from a sorted list of property names
  /// </summary>
  public static string KeyFromPropertyNames(
    IEnumerable<string> sortedPropertyNames)
  {
    var sb = new StringBuilder();
    sb.Append("{ ");
    foreach(var property in sortedPropertyNames)
    {
      if(property.IndexOfAny(__needQuoteCharacters)>=0)
      {
        sb.Append('(');
        sb.Append(property);
        sb.Append(')');
      }
      else
      {
        sb.Append(property);
      }
      sb.Append(' ');
    }
    sb.Append('}');
    return sb.ToString();
  }

  private string ValueToken(JValue value)
  {
    return value.Type switch {
      JTokenType.Null => "null",
      JTokenType.String => "string",
      JTokenType.Boolean => "boolean",
      JTokenType.Integer or JTokenType.Float => "number",
      _ => throw new NotSupportedException(
        $"Unsupported JSON token type {value.Type}"),
    };
  }

  private string ObjectToken(JObject o)
  {
    var propNames = ObjectFieldSet(o);
    return KeyFromPropertyNames(propNames);
  }
}
