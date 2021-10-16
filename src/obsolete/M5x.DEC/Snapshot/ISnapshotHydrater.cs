using M5x.Schemas;

namespace M5x.DEC.Snapshot
{
    public interface ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}