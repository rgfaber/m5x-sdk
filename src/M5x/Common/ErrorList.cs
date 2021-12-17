using System;
using System.Collections.Generic;
using M5x.Extensions;

namespace M5x.Common;

public class ErrorList : List<KeyValuePair<string, Xeption>>
{
    public void Add(string key, Xeption ex)
    {
        var element = new KeyValuePair<string, Xeption>(key, ex);
        base.Add(element);
    }

    public void Add(string key, Exception e)
    {
        Add(key, e.ToXeption());
    }

    public new void AddRange(IEnumerable<KeyValuePair<string, Xeption>> range)
    {
        base.AddRange(range);
    }
}