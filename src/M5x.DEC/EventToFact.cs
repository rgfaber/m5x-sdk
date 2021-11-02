using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;


namespace M5x.DEC
{
    public abstract class EventToFact<TID, TEvent, TFact> : IEventHandler<TID, TEvent> 
        where TID : IIdentity 
        where TEvent : IEvent<TID>
        where TFact: IFact
    {
        private readonly IDECBus _bus;

        protected EventToFact(IDECBus bus)
        {
            _bus = bus;
            _bus.Subscribe<TEvent>( evt=> HandleAsync(evt));
        }
        
        public Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            var fact = ToFact(@event);
            return _bus.PublishAsync(fact);
        }
        protected abstract TFact ToFact(TEvent @event);
    }
}