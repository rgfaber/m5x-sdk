using M5x.DEC.Schema;

namespace M5x.DEC.Snapshot
{
    public interface ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}