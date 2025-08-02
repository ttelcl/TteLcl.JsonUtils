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
/// Implements <see cref="SlotValue"/> for objects
/// </summary>
public class SlotValueObject: SlotValue
{
  private readonly Dictionary<string, SlotTracker> _propertyTrackers;

  /// <summary>
  /// Create a new SlotValueObject
  /// </summary>
  private SlotValueObject(
    SlotTracker host,
    string objectKey,
    IEnumerable<string> propertyNames)
    : base(host, objectKey)
  {
    _propertyTrackers = new Dictionary<string, SlotTracker>();
    foreach(var name in propertyNames)
    {
      var tracker = Host.CreatePropertySlot(name);
      _propertyTrackers[name] = tracker;
    }
  }

  /// <summary>
  /// Create a new SlotValueObject
  /// </summary>
  public static SlotValueObject Create(
    SlotTracker host, IEnumerable<string> propertyNames)
  {
    var propList = new List<string>(propertyNames);
    propList.Sort();
    var key = TrackerService.KeyFromPropertyNames(propList);
    return new SlotValueObject(host, key, propertyNames);
  }

  /// <summary>
  /// Mapping of the property names to the trackers handling the
  /// observed property values
  /// </summary>
  public IReadOnlyDictionary<string, SlotTracker> PropertyMap => _propertyTrackers;

  /// <summary>
  /// The collection of property value trackers
  /// </summary>
  public IReadOnlyCollection<SlotTracker> Properties => _propertyTrackers.Values;

  /// <inheritdoc/>
  protected override void ObserveImplementation(JToken token)
  {
    if(token is JObject o)
    {
      // First make sure there are no unexpected properties
      foreach(var tokenProp in o.Properties())
      {
        var propName = tokenProp.Name;
        if(!_propertyTrackers.ContainsKey(propName))
        {
          throw new InvalidOperationException(
            $"Internal error: received property '{propName}' for a slot that not expecting it");
        }
      }
      // Then process all values
      foreach(var kvp in _propertyTrackers)
      {
        var propName = kvp.Key;
        var tracker = kvp.Value;
        var value = token[propName];
        // Value may be null! But that's ok. It indicates a missing optional property.
        tracker.TrackValue(value);
      }
    }
    else
    {
      throw new InvalidOperationException(
        $"Internal error - this code path should only see objects");
    }
  }
}
