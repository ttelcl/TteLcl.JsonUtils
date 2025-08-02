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
/// A special <see cref="SlotValue"/> implementation for
/// missing optional properties (treated as a magical 'void' type)
/// </summary>
public class SlotValueVoid: SlotValue
{
  /// <summary>
  /// Create a new SlotValueVoid
  /// </summary>
  public SlotValueVoid(
    SlotTracker host)
    : base(host, "void")
  {
  }

  /// <summary>
  /// Override that always throws a <see cref="NotSupportedException"/>
  /// </summary>
  protected override void ObserveImplementation(JToken token)
  {
    throw new NotSupportedException(
      "This slot value does not expect actual JSON tokens");
  }

  /// <summary>
  /// Observe a missing property value. This doesn't do anything
  /// except disabling the error that would normally be thrown.
  /// </summary>
  protected override void ObserveVoid()
  {
    // no actual content beyond disabling the base implementation
  }

}
