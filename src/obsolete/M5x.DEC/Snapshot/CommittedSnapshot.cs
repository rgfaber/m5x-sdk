using System;
using M5x.Schemas;

namespace M5x.DEC.Snapshot
{
    public class CommittedSnapshot<TAggregate, TIdentity, TAggregateSnapshot>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateSnapshot : IAggregateSnapshot<TAggregate, TIdentity>
    {
        public CommittedSnapshot(
            TIdentity aggregateIdentity,
            TAggregateSnapshot aggregateSnapshot,
            SnapshotMetadata metadata,
            DateTimeOffset timestamp,
            long aggregateSequenceNumber)
        {
            if (aggregateSnapshot == null) throw new ArgumentNullException(nameof(aggregateSnapshot));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (timestamp == default) throw new ArgumentNullException(nameof(timestamp));
            if (aggregateIdentity == null || string.IsNullOrEmpty(aggregateIdentity.Value))
                throw new ArgumentNullException(nameof(aggregateIdentity));
            if (aggregateSequenceNumber <= 0) throw new ArgumentOutOfRangeException(nameof(aggregateSequenceNumber));

            AggregateIdentity = aggregateIdentity;
            AggregateSequenceNumber = aggregateSequenceNumber;
            AggregateIdentity = aggregateIdentity;
            AggregateSnapshot = aggregateSnapshot;
            Metadata = metadata;
            Timestamp = timestamp;
        }

        public TIdentity AggregateIdentity { get; }
        public TAggregateSnapshot AggregateSnapshot { get; }
        public SnapshotMetadata Metadata { get; }
        public long AggregateSequenceNumber { get; }
        public DateTimeOffset Timestamp { get; }
    }
}