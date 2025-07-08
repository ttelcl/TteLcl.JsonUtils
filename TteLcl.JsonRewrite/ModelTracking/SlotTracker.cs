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
/// Tracks JSON types appearing in a particular slot (the root,
/// a child of an array, the value of a property)
/// </summary>
public class SlotTracker
{
  private readonly Dictionary<string, SlotValue> _slotValueMap;

  /// <summary>
  /// Create a new SlotTracker
  /// </summary>
  private SlotTracker(
    SlotTracker? parent,
    TrackerService service,
    string key,
    bool literalName = false)
  {
    _slotValueMap = new Dictionary<string, SlotValue>();
    Parent = parent;
    Service = service;
    key = String.IsNullOrEmpty(key) ? "/" : key;
    if(!literalName && key.IndexOfAny("(){}/.[]".ToCharArray())>=0)
    {
      key = "(" + key + ")";
    }
    Key = key;
    if(parent != null)
    {
      Path = parent.Path + "." + key;
    }
    else
    {
      Path = key;
    }
  }

  /// <summary>
  /// Create the root slot
  /// </summary>
  public static SlotTracker CreateRootSlot(
    TrackerService service)
  {
    return new SlotTracker(null, service, "/", true);
  }

  /// <summary>
  /// Create a child slot to store the children observed in an array
  /// </summary>
  /// <returns></returns>
  public SlotTracker CreateArraySlot()
  {
    return new SlotTracker(this, Service, "[]", true);
  }

  /// <summary>
  /// Create a child slot to store values observed for a property
  /// </summary>
  public SlotTracker CreatePropertySlot(string propertyName)
  {
    return new SlotTracker(this, Service, propertyName, false);
  }

  /// <summary>
  /// The parent slot, or null for the root
  /// </summary>
  public SlotTracker? Parent { get; }

  /// <summary>
  /// The tracker service object. All trackers in the tree share the same
  /// tracker service (initially bound in <see cref="CreateRootSlot(TrackerService)"/>)
  /// </summary>
  public TrackerService Service { get; }

  /// <summary>
  /// The key (label) of this slot (relative to <see cref="Parent"/>)
  /// </summary>
  public string Key { get; }

  /// <summary>
  /// The full path to this slot starting from the root 
  /// </summary>
  public string Path { get; }

  /// <summary>
  /// A map that lists the types found in this slot (indexed by type name)
  /// </summary>
  public IReadOnlyDictionary<string, SlotValue> TypeSlotMap => _slotValueMap;

  /// <summary>
  /// The collection of types found in this slot
  /// </summary>
  public IReadOnlyCollection<SlotValue> TypeSlots => _slotValueMap.Values;

  /// <summary>
  /// Process the given token appearing at this slot
  /// </summary>
  public void TrackValue(JToken? token)
  {
    var typeKey = Service.TokenKey(token);
    if(!_slotValueMap.TryGetValue(typeKey, out var slotValue))
    {
      slotValue = SlotValue.CreateFor(this, token, typeKey);
      if(slotValue.TypeKey != typeKey)
      {
        throw new InvalidOperationException(
          $"Internal error: Expecting created slot key '{slotValue.TypeKey}' to match the given key '{typeKey}'");
      }
      _slotValueMap[typeKey] = slotValue;
    }
    slotValue.Observe(token);
  }

}
