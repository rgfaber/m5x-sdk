using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.PubSub
{
    public interface IBroadcaster<in TID> 
    where TID: IIdentity
    {
        public Task BroadcastAsync(IEvent<TID> evt);
    }

    public abstract class Broadcaster<TID> : IBroadcaster<TID> where TID : IIdentity
    {
        private readonly IDECBus _bus;
        protected Broadcaster(IDECBus bus)
        {
            _bus = bus;
            _bus.Subscribe<IEvent<TID>>(BroadcastAsync);
        }
        public abstract Task BroadcastAsync(IEvent<TID> evt);
    }













}