using M5x.DEC.Schema;

namespace M5x.DEC.Snapshot;

public interface IAggregateSnapshot : IVersionedType
{
}

public interface IAggregateSnapshot<TAggregate, TID> : IAggregateSnapshot
    where TAggregate : IAggregateRoot<TID>
    where TID : IIdentity
{
}