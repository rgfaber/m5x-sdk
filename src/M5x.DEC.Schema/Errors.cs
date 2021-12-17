using System;
using System.Collections.Generic;
using System.Text.Json;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Schema;

[Serializable]
public sealed class Errors : List<KeyValuePair<string, Error>>
{
    public void Add(string key, string message)
    {
        var err = new Error { ErrorMessage = message };
        Add(key, err);
    }

    public void Add(string key, Error ex)
    {
        var element = new KeyValuePair<string, Error>(key, ex);
        base.Add(element);
    }

    public void Add(string key, Exception e)
    {
        Add(key, e.AsApiError());
    }

    public new void AddRange(IEnumerable<KeyValuePair<string, Error>> range)
    {
        base.AddRange(range);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}