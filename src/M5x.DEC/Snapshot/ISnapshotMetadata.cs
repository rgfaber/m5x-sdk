using M5x.DEC.Core;

namespace M5x.DEC.Snapshot;

public interface ISnapshotMetadata : IMetadataContainer
{
    ISnapshotId SnapshotId { get; }
    string SnapshotName { get; }
    int SnapshotVersion { get; }
    long AggregateSequenceNumber { get; }
    string AggregateId { get; }
    string AggregateName { get; }
}