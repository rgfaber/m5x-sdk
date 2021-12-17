using System;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch.Lucene;

/// <summary>
///     A Lucene query hit containing document id and score.
///     Optionally it also contains field (optional), sort_order (optional) and the actual document embedded
///     if you used IncludeDocuments() when querying.
/// </summary>
public class CouchLuceneHit : ICanJson
{
    public JObject Obj;

    public CouchLuceneHit(JObject row)
    {
        Obj = row;
    }


    public void WriteJson(JsonWriter writer)
    {
        throw new NotImplementedException();
    }

    public void ReadJson(JObject obj)
    {
        Obj = obj;
    }

    public string Id()
    {
        return Obj["id"].Value<string>();
    }

    public float Score()
    {
        return Obj["score"].Value<float>();
    }

    public Array SortOrder()
    {
        return Obj["sort_order"].Value<Array>();
    }

    /// <summary>
    ///     Extract any embedded document.
    /// </summary>
    public T Document<T>() where T : ICouchDocument, new()
    {
        if (!HasDocument())
            throw new CouchException("No embedded document in this Lucene hit. Did you forget IncludeDocuments()?");
        var doc = new T();
        doc.ReadJson(Obj["doc"].Value<JObject>());
        return doc;
    }

    public bool HasDocument()
    {
        return Obj["doc"] != null;
    }
}