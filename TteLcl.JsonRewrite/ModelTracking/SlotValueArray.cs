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
/// Implements <see cref="SlotValue"/> for arrays
/// </summary>
public class SlotValueArray: SlotValue
{
  /// <summary>
  /// Maps child type keys to SlotValues
  /// </summary>
  private readonly Dictionary<string, SlotValue> _childMap;

  /// <summary>
  /// Create a new SlotValueArray
  /// </summary>
  public SlotValueArray(
    SlotTracker host)
    : base(host, "array")
  {
    _childMap = new Dictionary<string, SlotValue>();
  }

  /// <summary>
  /// A mapping from type keys to SlotValues that observed that type
  /// </summary>
  public IReadOnlyDictionary<string, SlotValue> ChildMap => _childMap;

  /// <summary>
  /// All the child values (shorthand ChildMap.Values)
  /// </summary>
  public IReadOnlyCollection<SlotValue> Children => _childMap.Values;

  /// <summary>
  /// The shortest observed array length for this slot
  /// </summary>
  public int MinLength { get; private set; } = Int32.MaxValue;

  /// <summary>
  /// The longest observed array length for this slot
  /// </summary>
  public int MaxLength { get; private set; } = 0;

  /// <summary>
  /// Handles JSON arrays in the input
  /// </summary>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JArray a)
    {
      var len = a.Count;
      if(len < MinLength)
      {
        MinLength = len;
      }
      if(len > MaxLength)
      {
        MaxLength = len;
      }
      var trackerService = Host.Service;
      foreach(var child in a)
      {
        var key = trackerService.TokenKey(child);
        if(!_childMap.TryGetValue(key, out var slotValue))
        {
          slotValue = SlotValue.CreateFor(Host, child, key);
          if(slotValue.TypeKey != key)
          {
            throw new InvalidOperationException(
              $"Internal error: Expecting created slot key '{slotValue.TypeKey}' to match the given key '{key}'");
          }
          _childMap[key] = slotValue;
        }
        slotValue.Observe(child);
      }
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see JSON array values");
    }
  }
}
