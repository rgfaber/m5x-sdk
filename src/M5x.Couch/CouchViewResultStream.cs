using System;
using System.Collections;
using System.Collections.Generic;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch;

/// <summary>
///     This is a view result from a CouchQuery that can return CouchDocuments for
///     resulting documents (include_docs) and/or ICanJson documents for the
///     result values. A value returned from a CouchDB view does not need to be
///     a CouchDocument.
/// </summary>
public class CouchViewResultStream<T> : CouchViewResult, IEnumerable<CouchRecord<T>>, IDisposable
    where T : ICanJson, new()
{
    private readonly JsonReader _reader;
    private bool _hasMore = true;

    public CouchViewResultStream(JsonReader reader)
    {
        _reader = reader;

        var header = new JObject();

        // start object
        reader.Read();

        while (reader.Read() && reader.TokenType != JsonToken.StartArray)
        {
            var name = reader.Value.ToString();
            if (name == "rows")
                continue;

            reader.Read();
            header[name] = new JValue(reader.Value);
        }

        reader.Read();
    }

    public void Dispose()
    {
        _reader.Close();
    }

    public IEnumerator<CouchRecord<T>> GetEnumerator()
    {
        if (!_hasMore)
            throw new InvalidOperationException("Result stream cannot be re-enumerated");

        return new RecordEnumerator(_reader, this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        if (!_hasMore)
            throw new InvalidOperationException("Result stream cannot be re-enumerated");

        return new RecordEnumerator(_reader, this);
    }

    private class RecordEnumerator : IEnumerator<CouchRecord<T>>
    {
        private readonly CouchViewResultStream<T> _owner;
        private readonly JsonReader _reader;
        private bool _hasMore = true;

        public RecordEnumerator(JsonReader reader, CouchViewResultStream<T> owner)
        {
            _reader = reader;
            _owner = owner;
        }

        public CouchRecord<T> Current { get; private set; }

        public void Dispose()
        {
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (!_hasMore)
                return false;

            var token = JToken.ReadFrom(_reader);
            _hasMore = _reader.Read() && _reader.TokenType == JsonToken.StartObject;
            _owner._hasMore = _hasMore;

            Current = new CouchRecord<T>(token as JObject);

            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}