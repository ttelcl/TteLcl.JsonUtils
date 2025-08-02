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
/// Implements <see cref="SlotValue"/> for JSON nulls
/// </summary>
public class SlotValueNull: SlotValue
{
  /// <summary>
  /// Create a new SlotValueNull
  /// </summary>
  public SlotValueNull(
    SlotTracker host)
    : base(host, "null")
  {
  }

  /// <inheritdoc/>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JValue v && v.Type == JTokenType.Null)
    {
      // nothing to do
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see JSON null values");
    }
  }

}
