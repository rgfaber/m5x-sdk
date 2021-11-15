using System;
using System.Collections.Generic;
using System.Reflection;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence
{
    [Obsolete("Please use AsyncEventStream Instead")]
    public abstract class EventStream<TAggregate, TAggregateId>
        : IEventStream<TAggregate, TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>, IAggregate<TAggregateId>
        where TAggregateId : IIdentity
    {
        private readonly IDECBus bus;

        private readonly IEventStore eventStore;

        public EventStream(IEventStore eventStore, IDECBus bus)
        {
            this.eventStore = eventStore;
            this.bus = bus;
        }


        public TAggregate GetById(TAggregateId id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate(id);
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;
                var lst = eventStore.ReadEventsAsync(id).Result;
                if (lst == null) return aggregate;
                foreach (var @event in lst)
                    aggregatePersistence.ApplyEvent(@event.Event, @event.EventNumber);
                return aggregate;
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }


        public IEnumerable<StoreEvent<TAggregateId>> Load(TAggregateId id)
        {
            try
            {
                return eventStore.ReadEventsAsync(id).Result;
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }


        public async void Save(TAggregate aggregate)
        {
            try
            {
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;
                var uncomEvts = aggregatePersistence.GetUncommittedEvents();
                foreach (var @event in uncomEvts)
                {
                    await eventStore.AppendEventAsync(@event);
                    await bus.PublishAsync((dynamic)@event);
                }

                aggregatePersistence.ClearUncommittedEvents();
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }

        private TAggregate CreateEmptyAggregate(TAggregateId id)
        {
            try
            {
                var ci = typeof(TAggregate)
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                        null,
                        new Type[0],
                        new ParameterModifier[0]);
                var res = ci.Invoke(new object[0]);
                var result = (TAggregate)res;
                result.Id = id;
                return result;
            }
            catch (Exception e)
            {
                throw new RepositoryException("Failed at Creating an Empty Aggregate!", e);
            }
        }
    }

    public interface IEventStream
    {
    }


    [Obsolete("Please use AsyncEventStream instead")]
    public interface IEventStream<TAggregate, TAggregateId> : IEventStream
        where TAggregate : IAggregate<TAggregateId>
        where TAggregateId : IIdentity
    {
        TAggregate GetById(TAggregateId id);

        void Save(TAggregate aggregate);

        IEnumerable<StoreEvent<TAggregateId>> Load(TAggregateId id);
    }
}