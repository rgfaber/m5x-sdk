using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence;

public interface IFactHandler<TAggregateId, TFact>
    where TFact : IFact
    where TAggregateId : IIdentity
{
    Task HandleAsync(TFact fact);
}

public interface IFactWriter<TAggregateId, TFact, TReadModel>
    : IFactHandler<TAggregateId, TFact>
    where TReadModel : IReadEntity
    where TFact : IFact
    where TAggregateId : IIdentity
{
    Task<TReadModel> UpdateAsync(TFact fact);
    Task<TReadModel> DeleteAsync(string id);
}

public interface IEventWriter<TID, TEvent, TModel> : IEventHandler<TID, TEvent>
    where TEvent : IEvent<TID>
    where TID : IIdentity
    where TModel : IReadEntity
{
    Task<TModel> UpdateAsync(TEvent evt);
    Task<TModel> DeleteAsync(string id);
}

public interface IEtlWriter<TID, in TEvent, TModel> : IEventHandler<TID, TEvent>
    where TEvent : IEvent<TID>
    where TModel : IReadEntity
    where TID : IIdentity
{
}