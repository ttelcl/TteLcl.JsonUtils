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
/// Implements <see cref="SlotValue"/> for integers and
/// floating point numbers
/// </summary>
public class SlotValueNumber: SlotValue
{
  /// <summary>
  /// Create a new SlotValueNumber
  /// </summary>
  public SlotValueNumber(
    SlotTracker host)
    : base(host, "number")
  {
  }

  /// <summary>
  /// The smallest observed value
  /// </summary>
  public double MinValue { get; private set; } = Double.MaxValue;

  /// <summary>
  /// The larges observed value
  /// </summary>
  public double MaxValue { get; private set; } = Double.MinValue;

  /// <summary>
  /// Handles processing of integers and floating point numbers
  /// </summary>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JValue v)
    {
      var value =
        token.Type switch {
          JTokenType.Float => (double)v,
          JTokenType.Integer => (double)(long)v,
          _ =>
            throw new InvalidOperationException(
              $"Internal error - this code path should only see numbers"),
        };
      if(value < MinValue)
      {
        MinValue = value;
      }
      if(value > MaxValue)
      {
        MaxValue = value;
      }
    }
  }
}
