using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.PubSub
{
    public abstract class Collector<TID> : ICollector<TID>
        where TID : IIdentity
    {
        private readonly IDECBus _bus;

        protected Collector(IDECBus bus)
        {
            _bus = bus;
            _bus.Subscribe<IEvent<TID>>(CollectAsync);
        }

        public abstract Task CollectAsync(IEvent<TID> evt);
    }

    public interface ICollector<in TID>
        where TID : IIdentity
    {
        Task CollectAsync(IEvent<TID> evt);
    }
}