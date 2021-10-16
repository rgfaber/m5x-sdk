using M5x.Schemas;

namespace M5x.DEC.Sagas.AggregateSaga
{
    public interface IAggregateSaga<TIdentity> : IAggregateRoot<TIdentity>
        where TIdentity : ISagaId, IIdentity
    {
    }
}