using System.Threading.Tasks;
using M5x.DEC.Schema;

namespace M5x.DEC.Sagas
{
    public interface ISagaHandles<TAggregate, in TIdentity, in TAggregateEvent> : ISaga
        where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        bool Handle(IDomainEvent<TAggregate, TIdentity, TAggregateEvent> domainEvent);
    }

    public interface ISagaHandlesAsync<TAggregate, in TIdentity, in TAggregateEvent> : ISaga
        where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        Task HandleAsync(IDomainEvent<TAggregate, TIdentity, TAggregateEvent> domainEvent);
    }
}