/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.JsonRewrite.ModelTracking.Exporting;

/// <summary>
/// Base class for exporters that can export a <see cref="SlotTracker"/>
/// + <see cref="SlotValue"/> based model of JSON data to a file.
/// </summary>
public abstract class JsonModelExporter
{
  /// <summary>
  /// Create a new JsonModelExporter
  /// </summary>
  protected JsonModelExporter()
  {
  }

  /// <summary>
  /// Export a model to a file.
  /// </summary>
  /// <param name="fileName">
  /// The name of the file to export to.
  /// </param>
  /// <param name="model">
  /// The model to export
  /// </param>
  public abstract void Export(string fileName, SlotTracker model);

}
