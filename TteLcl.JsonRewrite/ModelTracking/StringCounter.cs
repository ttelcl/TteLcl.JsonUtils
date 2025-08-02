/*
 * (c) 2025  ttelcl / ttelcl
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.JsonRewrite.ModelTracking;

/// <summary>
/// Description of StringCounter
/// </summary>
public class StringCounter
{
  private readonly Dictionary<string, int> _countMap;

  /// <summary>
  /// Create a new StringCounter
  /// </summary>
  public StringCounter(
    bool caseSensitive)
  {
    _countMap =
      caseSensitive
      ? new Dictionary<string, int>(StringComparer.Ordinal)
      : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
  }

  /// <summary>
  /// Increment the counter value for the key by 1
  /// (potentially creating the counter for that value)
  /// </summary>
  /// <param name="key">
  /// The key to count
  /// </param>
  /// <returns>
  /// Returns the new value
  /// </returns>
  public int Increment(string key)
  {
    if(_countMap.TryGetValue(key, out var count))
    {
      count++;
      if(count == 0)
      {
        _countMap.Remove(key);
        return 0;
      }
      _countMap[key] = count;
      return count;
    }
    else
    {
      _countMap[key] = 1;
      return 1;
    }
  }

  /// <summary>
  /// Get the collection of keys
  /// </summary>
  public IReadOnlyCollection<string> Keys => _countMap.Keys;

  /// <summary>
  /// Get the underlying count map
  /// </summary>
  public IReadOnlyDictionary<string, int> Map => _countMap;

  /// <summary>
  /// The number of distinct keys
  /// </summary>
  public int Count => _countMap.Count;

  /// <summary>
  /// Get or set the count. 
  /// </summary>
  public int this[string key] {
    get => GetCount(key);
    set => SetCount(key, value);
  }

  /// <summary>
  /// Get the count for the given key string (0 if none)
  /// </summary>
  public int GetCount(string key)
  {
    return _countMap.TryGetValue(key, out var value) ? value : 0;
  }

  /// <summary>
  /// Set the count for the key. Setting a count to 0 removes the
  /// entry
  /// </summary>
  public void SetCount(string key, int value)
  {
    if(value==0)
    {
      _countMap.Remove(key);
    }
    else
    {
      _countMap[key] = value;
    }
  }

  /// <summary>
  /// Remove all entries
  /// </summary>
  public void Clear()
  {
    _countMap.Clear();
  }

}
