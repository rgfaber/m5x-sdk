using System.Threading.Tasks;
using M5x.Schemas;

namespace M5x.DEC.Persistence
{
    public interface IModelWriter<TAggregateId, TEvent, TReadModel> : IAggregateEventHandler<TAggregateId, TEvent>
        where TReadModel : IReadEntity
        where TEvent : IEvent<TAggregateId>
        where TAggregateId : IAggregateID
    {
        Task<TReadModel> UpdateAsync(TEvent @event);
    }
}