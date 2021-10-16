using M5x.Schemas;

namespace M5x.DEC.Sagas
{
    public interface IEventApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
    }
}