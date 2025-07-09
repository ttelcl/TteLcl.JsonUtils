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
/// Handles observations of string values
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
    TrackedValues = new StringCounter(true);
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
  /// Tracks individual values, or null after heuristics decide to disable
  /// this (starts out non-null)
  /// </summary>
  public StringCounter? TrackedValues { get; private set; }

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
      if(TrackedValues != null)
      {
        if(txt.Length > 80
          || txt.IndexOfAny(__showstopperChars)>=0 )
        {
          // stop tracking
          TrackedValues = null;
        }
        else
        {
          TrackedValues.Increment(txt);
          if(TrackedValues.Count > 15)
          {
            // stop tracking
            TrackedValues = null;
          }
        }
      }
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see strings");
    }
  }
  private static char[] __showstopperChars = "\r\n".ToCharArray();
}
