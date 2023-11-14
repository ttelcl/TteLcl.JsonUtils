/*
 * (c) 2023  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TteLcl.JsonRewrite.JsonToJsonMapping;

/// <summary>
/// Describes a model for mapping JSON content to a smaller, simplified
/// version if the data's model
/// </summary>
public class JsonMapModel
{
  private JToken _model;

  /// <summary>
  /// Create a new JsonMapModel
  /// </summary>
  public JsonMapModel(JToken model)
  {
    _model = model;
  }

  /// <summary>
  /// Map the given data to this model
  /// </summary>
  /// <param name="data">
  /// The JSON data to map
  /// </param>
  /// <returns>
  /// The mapped data
  /// </returns>
  public JToken MapData(JToken data)
  {
    throw new NotImplementedException();
  }

}
