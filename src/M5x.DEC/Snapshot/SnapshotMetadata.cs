using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using M5x.DEC.Core;

namespace M5x.DEC.Snapshot;

public class SnapshotMetadata : MetadataContainer, ISnapshotMetadata
{
    public SnapshotMetadata()
    {
    }

    public SnapshotMetadata(IDictionary<string, string> keyValuePairs)
        : base(keyValuePairs)
    {
    }

    public SnapshotMetadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
    {
    }

    public SnapshotMetadata(params KeyValuePair<string, string>[] keyValuePairs)
        : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
    {
    }

    [JsonIgnore]
    public string AggregateId
    {
        get => GetMetadataValue(SnapshotMetadataKeys.AggregateId);
        set => Add(SnapshotMetadataKeys.AggregateId, value);
    }

    [JsonIgnore]
    public string AggregateName
    {
        get => GetMetadataValue(SnapshotMetadataKeys.AggregateName);
        set => Add(SnapshotMetadataKeys.AggregateName, value);
    }

    [JsonIgnore]
    public long AggregateSequenceNumber
    {
        get => GetMetadataValue(SnapshotMetadataKeys.AggregateSequenceNumber, long.Parse);
        set => Add(SnapshotMetadataKeys.AggregateSequenceNumber, value.ToString(CultureInfo.InvariantCulture));
    }

    [JsonIgnore]
    public string SnapshotName
    {
        get => GetMetadataValue(SnapshotMetadataKeys.SnapshotName);
        set => Add(SnapshotMetadataKeys.SnapshotName, value);
    }

    [JsonIgnore]
    public ISnapshotId SnapshotId
    {
        get => GetMetadataValue(SnapshotMetadataKeys.SnapshotId, Snapshot.SnapshotId.With);
        set => Add(SnapshotMetadataKeys.SnapshotId, value.Value);
    }

    [JsonIgnore]
    public int SnapshotVersion
    {
        get => GetMetadataValue(SnapshotMetadataKeys.SnapshotVersion, int.Parse);
        set => Add(SnapshotMetadataKeys.SnapshotVersion, value.ToString(CultureInfo.InvariantCulture));
    }
}