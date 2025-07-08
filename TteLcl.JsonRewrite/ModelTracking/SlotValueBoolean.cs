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
/// Implements <see cref="SlotValue"/> for booleans
/// </summary>
public class SlotValueBoolean: SlotValue
{
  /// <summary>
  /// Create a new SlotValueBoolean
  /// </summary>
  public SlotValueBoolean(
    SlotTracker host)
    : base(host, "boolean")
  {
  }

  /// <summary>
  /// The number of observed 'false' values
  /// </summary>
  public int FalseCount { get; private set; }

  /// <summary>
  /// The number of observed 'true' values
  /// </summary>
  public int TrueCount { get; private set; }

  /// <inheritdoc/>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JValue v && v.Type == JTokenType.Boolean)
    {
      var value = (bool)token;
      if(value)
      {
        TrueCount++;
      }
      else
      {
        FalseCount++;
      }
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see boolean values");
    }
  }

}
