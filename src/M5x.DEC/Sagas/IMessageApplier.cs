using M5x.DEC.Schema;
using M5x.DEC.Snapshot;

namespace M5x.DEC.Sagas;

public interface IMessageApplier<TAggregate, TIdentity> : IEventApplier<TAggregate, TIdentity>,
    ISnapshotHydrater<TAggregate, TIdentity>
    where TAggregate : IAggregateRoot<TIdentity>
    where TIdentity : IIdentity
{
}