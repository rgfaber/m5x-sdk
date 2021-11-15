using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<TID, in TEvent> : IEventHandler
        where TID : IIdentity
        where TEvent : IEvent<TID>
    {
        Task HandleAsync(TEvent @event);
    }
}