using System;
using System.Collections.Generic;
using System.Linq;
using M5x.DEC.Persistence;
using M5x.Schemas;
using M5x.Schemas.Commands;

namespace M5x.DEC
{
    
    public interface IStateEntity<TId> : IReadEntity
        where TId:IAggregateID
    {
        TId AggregateId { get; set; }
    }


    public abstract class AggregateRoot<TId, TState> : AggregateRoot<TId> 
        where TId : IAggregateID
        where TState: IStateEntity<TId>
    
    {
        protected TState State { get; }
    }
    
    
    
    public abstract class AggregateRoot<TId> : IAggregateRoot<TId>, 
        IEventSourcingAggregate<TId>
        where TId : IAggregateID
    {
        public const long NewAggregateVersion = -1;

        private readonly ICollection<IEvent<TId>> _uncommittedEvents = new LinkedList<IEvent<TId>>();

        private ISourceID _sourceId;
        
        private long _version = NewAggregateVersion;

        protected AggregateRoot(TId id)
        {
            Id = id;
        }
        
        protected AggregateRoot() {}

        public TId Id { get; set; }

        public IAggregateName Name { get; }
        public long Version { get; }
        public bool IsNew { get; }

        public bool HasSourceId(ISourceID sourceId)
        {
            return _sourceId == sourceId;
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        long IEventSourcingAggregate<TId>.Version => _version;

        void IEventSourcingAggregate<TId>.ApplyEvent(IEvent<TId> @event, long version)
        {
            if (_uncommittedEvents.Any(x => Equals(x.EventId, @event.EventId))) return;
            ((dynamic) this).Apply((dynamic) @event);
            _version = version;
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public IEnumerable<IEvent<TId>> GetUncommittedEvents()
        {
            return _uncommittedEvents.AsEnumerable();
        }

        public void Load(IEnumerable<IEvent<TId>> aggregateEvents)
        {
            ClearUncommittedEvents();
            foreach (var @event in aggregateEvents) _uncommittedEvents.Add(@event);
        }

        protected void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : Event<TId>
        {
            var eventWithAggregate = @event.WithAggregate(
                Equals(Id, default(TId)) ? @event.AggregateId : Id,
                _version);
            ((IEventSourcingAggregate<TId>) this).ApplyEvent(eventWithAggregate, _version + 1);
            _uncommittedEvents.Add(eventWithAggregate);
        }
        
        public bool ValidateState(params Func<IAggregateRoot,bool>[] validations)
        {
            return validations == null ||
                   validations.Aggregate(true,
                       (current, validation) => current && validation(this));
        }

        
    }
}