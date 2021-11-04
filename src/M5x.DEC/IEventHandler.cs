using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IEventHandler {}

    public interface IEventHandler<TAggregateId, in TEvent> : IEventHandler
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
    {
        Task HandleAsync(TEvent @event);
    }

}