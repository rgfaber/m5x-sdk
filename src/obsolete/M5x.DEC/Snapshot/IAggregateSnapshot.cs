using M5x.Schemas;

namespace M5x.DEC.Snapshot
{
    public interface IAggregateSnapshot : IVersionedType
    {
    }

    public interface IAggregateSnapshot<TAggregate, TIdentity> : IAggregateSnapshot
        where TAggregate : IAggregateRoot<TIdentity> where TIdentity : IIdentity
    {
    }
}