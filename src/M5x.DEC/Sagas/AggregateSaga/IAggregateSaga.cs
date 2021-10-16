using M5x.DEC.Schema;

namespace M5x.DEC.Sagas.AggregateSaga
{
    public interface IAggregateSaga<TIdentity> : IAggregateRoot<TIdentity>
        where TIdentity : ISagaId, IIdentity
    {
    }
}