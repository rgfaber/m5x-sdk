using System;
using System.Collections.Generic;
using System.Linq;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch;

/// <summary>
///     Only used as psuedo doc when doing bulk updates/inserts.
/// </summary>
public class CouchBulkDocuments : ICanJson
{
    public CouchBulkDocuments(IEnumerable<ICouchDocument> docs)
    {
        Docs = docs;
    }

    public IEnumerable<ICouchDocument> Docs { get; }

    #region ICouchBulk Members

    public int Count()
    {
        return Docs.Count();
    }

    public virtual void WriteJson(JsonWriter writer)
    {
        writer.WritePropertyName("docs");
        writer.WriteStartArray();
        foreach (var doc in Docs)
            if (doc is ISelfContained)
            {
                doc.WriteJson(writer);
            }
            else
            {
                writer.WriteStartObject();
                doc.WriteJson(writer);
                writer.WriteEndObject();
            }

        writer.WriteEndArray();
    }

    public virtual void ReadJson(JObject obj)
    {
        throw new NotImplementedException();
    }

    #endregion
}