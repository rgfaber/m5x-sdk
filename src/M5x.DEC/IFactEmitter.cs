using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IFactEmitter
    {
        public string Topic { get; }
    }
    
    public interface IFactEmitter<TAggregateId, TEvent, in TFact>
        : IEventHandler<TAggregateId, TEvent>, IFactEmitter
        where TAggregateId : IIdentity
        where TFact : IFact
        where TEvent : IEvent<TAggregateId>
    {
        
    }
}