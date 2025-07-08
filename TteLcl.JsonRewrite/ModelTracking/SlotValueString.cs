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
/// Description of SlotValueString
/// </summary>
public class SlotValueString: SlotValue
{
  /// <summary>
  /// Create a new SlotValueString
  /// </summary>
  public SlotValueString(
    SlotTracker host)
    : base(host, "string")
  {
  }

  /// <summary>
  /// Shortest observed string
  /// </summary>
  public int MinLength { get; private set; } = Int32.MaxValue;

  /// <summary>
  /// Longest observed string
  /// </summary>
  public int MaxLength { get; private set; } = 0;

  /// <summary>
  /// Process an observed string
  /// </summary>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JValue value && value.Type == JTokenType.String)
    {
      var txt = (string)value!;
      var txtLength = txt.Length;
      if(txtLength < MinLength)
      {
        MinLength = txtLength;
      }
      if(txtLength > MaxLength)
      {
        MaxLength = txtLength;
      }
      // Todo: heuristics to find 'common' values
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see strings");
    }
  }
}
