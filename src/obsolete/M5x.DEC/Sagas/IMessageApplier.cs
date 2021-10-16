using M5x.DEC.Snapshot;
using M5x.Schemas;

namespace M5x.DEC.Sagas
{
    public interface IMessageApplier<TAggregate, TIdentity> : IEventApplier<TAggregate, TIdentity>,
        ISnapshotHydrater<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}