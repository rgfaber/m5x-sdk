using System.Threading.Tasks;
using M5x.Schemas;

namespace M5x.DEC
{
    public interface IAggregateEventHandler<TAggregateId, TEvent>
        where TEvent : IEvent<TAggregateId>
    {
        Task HandleAsync(TEvent @event);
    }
}