using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using M5x.DEC.Commands;
using M5x.DEC.Core;
using M5x.DEC.Extensions;

namespace M5x.DEC.Sagas;

public class Metadata : MetadataContainer, IMetadata
{
    public Metadata()
    {
        // Empty
    }

    public Metadata(IDictionary<string, string> keyValuePairs)
        : base(keyValuePairs)
    {
    }

    public Metadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
    {
    }

    public Metadata(params KeyValuePair<string, string>[] keyValuePairs)
        : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
    {
    }

    public static IMetadata Empty { get; } = new Metadata();

    [JsonIgnore]
    public string AggregateName
    {
        get => GetMetadataValue(MetadataKeys.AggregateName);
        set => Add(MetadataKeys.AggregateName, value);
    }

    public ISourceID SourceId
    {
        get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceID(v)); }
        set => Add(MetadataKeys.SourceId, value.Value);
    }

    [JsonIgnore]
    public string EventName
    {
        get => GetMetadataValue(MetadataKeys.EventName);
        set => Add(MetadataKeys.EventName, value);
    }

    [JsonIgnore]
    public int EventVersion
    {
        get => GetMetadataValue(MetadataKeys.EventVersion, int.Parse);
        set => Add(MetadataKeys.EventVersion, value.ToString());
    }

    [JsonIgnore]
    public DateTimeOffset Timestamp
    {
        get => GetMetadataValue(MetadataKeys.Timestamp, DateTimeOffset.Parse);
        set => Add(MetadataKeys.Timestamp, value.ToString("O"));
    }

    [JsonIgnore]
    public long TimestampEpoch
    {
        get
        {
#pragma warning disable IDE0018 // Inline variable declaration
            string timestampEpoch;
#pragma warning restore IDE0018 // Inline variable declaration
            return TryGetValue(MetadataKeys.TimestampEpoch, out timestampEpoch)
                ? long.Parse(timestampEpoch)
                : Timestamp.ToUnixTime();
        }
    }

    [JsonIgnore]
    public long AggregateSequenceNumber
    {
        get => GetMetadataValue(MetadataKeys.AggregateSequenceNumber, int.Parse);
        set => Add(MetadataKeys.AggregateSequenceNumber, value.ToString());
    }

    [JsonIgnore]
    public string AggregateId
    {
        get => GetMetadataValue(MetadataKeys.AggregateId);
        set => Add(MetadataKeys.AggregateId, value);
    }

    [JsonIgnore]
    public string CorrelationId
    {
        get => GetMetadataValue(MetadataKeys.CorrelationId);
        set => Add(MetadataKeys.CorrelationId, value);
    }

    [JsonIgnore]
    public string CausationId
    {
        get => GetMetadataValue(MetadataKeys.CausationId);
        set => Add(MetadataKeys.CausationId, value);
    }

    [JsonIgnore]
    public IEventId EventId
    {
        get => GetMetadataValue(MetadataKeys.EventId, Sagas.EventId.With);
        set => Add(MetadataKeys.EventId, value.Value);
    }

    public IMetadata CloneWith(params KeyValuePair<string, string>[] keyValuePairs)
    {
        return CloneWith((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);
    }

    public IMetadata CloneWith(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        var metadata = new Metadata(this);
        foreach (var kv in keyValuePairs)
        {
            if (metadata.ContainsKey(kv.Key)) throw new ArgumentException($"Key '{kv.Key}' is already present!");
            metadata[kv.Key] = kv.Value;
        }

        return metadata;
    }

    public static IMetadata With(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        return new Metadata(keyValuePairs);
    }

    public static IMetadata With(params KeyValuePair<string, string>[] keyValuePairs)
    {
        return new Metadata(keyValuePairs);
    }

    public static IMetadata With(IDictionary<string, string> keyValuePairs)
    {
        return new Metadata(keyValuePairs);
    }
}