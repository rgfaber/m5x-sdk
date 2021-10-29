using System.Collections.Generic;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IAggregate<TId> 
        where TId : IIdentity
    {
        TId Id { get; }
        void Load(IEnumerable<IEvent<TId>> aggregateEvents);
        IEnumerable<IEvent<TId>> GetUncommittedEvents();
    }
}