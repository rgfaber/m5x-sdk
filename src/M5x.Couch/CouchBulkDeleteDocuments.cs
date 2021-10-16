using System;
using System.Collections.Generic;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch
{
    /// <summary>
    ///     Only used as pseudo doc when doing bulk updates/inserts.
    /// </summary>
    public class CouchBulkDeleteDocuments : CouchBulkDocuments
    {
        public CouchBulkDeleteDocuments(IEnumerable<ICouchDocument> docs) : base(docs)
        {
        }

        public override void WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("docs");
            writer.WriteStartArray();
            foreach (var doc in Docs)
            {
                writer.WriteStartObject();
                CouchDocument.WriteIdAndRev(doc, writer);
                writer.WritePropertyName("_deleted");
                writer.WriteValue(true);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override void ReadJson(JObject obj)
        {
            throw new NotImplementedException();
        }
    }
}