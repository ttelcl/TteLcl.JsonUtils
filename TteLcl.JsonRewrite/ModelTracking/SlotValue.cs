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
/// Gathers statistics and information on a value of a specific type
/// in a specific slot
/// </summary>
public abstract class SlotValue
{
  /// <summary>
  /// Create a new SlotValue
  /// </summary>
  protected SlotValue(
    SlotTracker host,
    string typekey)
  {
    Host = host;
    TypeKey = typekey;
  }

  /// <summary>
  /// Create a <see cref="SlotValue"/> subclass instance for the given
  /// sample
  /// </summary>
  /// <param name="host">
  /// The tracker that will host the new slot
  /// </param>
  /// <param name="sample">
  /// The sample determining the type to create. Null indicates
  /// the 'void' type used for missing object properties.
  /// </param>
  /// <param name="typeKey">
  /// Optional: the precalculated type key (which must match <paramref name="sample"/>)
  /// If null, it will be calculated from <paramref name="sample"/>.
  /// </param>
  /// <returns>
  /// A new <see cref="SlotValue"/> subclass instance.
  /// </returns>
  public static SlotValue CreateFor(
    SlotTracker host,
    JToken? sample,
    string? typeKey)
  {
    // sample == null in case of the 'void' type (a missing optional property)
    if(sample == null)
    {
      return new SlotValueVoid(host);
    }
    return sample switch {
      JObject o => SlotValueObject.Create(host, host.Service.ObjectFieldSet(o)),
      JArray _ => new SlotValueArray(host),
      JValue v =>
        v.Type switch {
          JTokenType.String => new SlotValueString(host),
          JTokenType.Integer or JTokenType.Float => new SlotValueNumber(host),
          JTokenType.Boolean => new SlotValueBoolean(host),
          _ => throw new InvalidOperationException(
            $"Unrecognized JSON value type '{v.Type}'"),
        },
      _ => throw new InvalidOperationException(
        $"Unexpected JSON sample type ({sample.Type})"),
    };
  }

  /// <summary>
  /// The slot this value is in
  /// </summary>
  public SlotTracker Host { get; }

  /// <summary>
  /// The name of the Json type handled by this slot. Normally
  /// generated via <see cref="TrackerService.TokenKey(JToken)"/>
  /// </summary>
  public string TypeKey { get; }

  /// <summary>
  /// The number of tokens of this type observed
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  /// Process the observation of the token. The token must match
  /// the expectation of the implementation class. Most of the
  /// processing is handled in <see cref="ObserveImplementation(JToken)"/>.
  /// </summary>
  /// <param name="token">
  /// The JSON token to process, or null to indicate a missing property.
  /// </param>
  /// <remarks>
  /// <para>
  /// It is up to the implementation to decide what to track. For example,
  /// the implementation for numbers may track minimum and maximum observed
  /// value. The implementations for objects and arrays should recurse.
  /// </para>
  /// </remarks>
  public void Observe(JToken? token)
  {
    if(token == null)
    {
      ObserveVoid();
    }
    else
    {
      ObserveImplementation(token);
    }
    Count++;
  }

  /// <summary>
  /// Update information in this value based on the observation of
  /// the given value.
  /// </summary>
  /// <param name="token">
  /// The observed token. This must be compatible with the implementation
  /// type.
  /// </param>
  protected abstract void ObserveImplementation(JToken token);

  /// <summary>
  /// Handle a missing object property ('void' type). Most implementations
  /// do not support this case. This default implementation throws a
  /// <see cref="NotSupportedException"/>.
  /// </summary>
  protected virtual void ObserveVoid()
  {
    throw new NotSupportedException(
      "This implementation does not support the 'void' type (missing object properties)");
  }
}
