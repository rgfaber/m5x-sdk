using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using M5x.Schemas;

namespace M5x.DEC.Persistence
{
    public abstract class EventRepository<TAggregate, TAggregateId> : IEventRepository<TAggregate, TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>, IAggregate<TAggregateId>
        where TAggregateId : IAggregateID
    {
        private readonly IEventStore eventStore;
        private readonly IAggregatePublisher publisher;

        public EventRepository(IEventStore eventStore, IAggregatePublisher publisher)
        {
            this.eventStore = eventStore;
            this.publisher = publisher;
        }

        public async Task<TAggregate> GetByIdAsync(TAggregateId id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate();
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;
                var lst = await eventStore.ReadEventsAsync(id);
                if (lst == null) return aggregate;
                foreach (var @event in lst)
                    aggregatePersistence.ApplyEvent(@event.Event, @event.EventNumber);
                return aggregate;
            }
            // catch (EventStoreAggregateNotFoundException)
            // {
            //     return CreateEmptyAggregate();
            // }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }


        public async Task<IEnumerable<StoreEvent<TAggregateId>>> LoadAsync(TAggregateId id)
        {
            try
            {
                return await eventStore.ReadEventsAsync(id);
            }
            // catch (EventStoreAggregateNotFoundException)
            // {
            //     return null;
            // }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }


        public async Task SaveAsync(TAggregate aggregate)
        {
            try
            {
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;
                var uncomEvts = aggregatePersistence.GetUncommittedEvents();
                foreach (var @event in uncomEvts)
                {
                    await eventStore.AppendEventAsync(@event);
                    await publisher.PublishAsync((dynamic) @event);
                }

                aggregatePersistence.ClearUncommittedEvents();
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }

        private TAggregate CreateEmptyAggregate()
        {
            try
            {
                var ci =  typeof(TAggregate)
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, 
                        null, 
                        new Type[0], 
                        new ParameterModifier[0]);
                var res = ci.Invoke(new object[0]);
                return (TAggregate)res;
            }
            catch (Exception e)
            {
                // return default;
                throw new RepositoryException("Failed at Creating an Empty Aggregate!", e);
            }
        }
    }


    public interface IEventRepository<TAggregate, TAggregateId>
        where TAggregate : IAggregate<TAggregateId>
    {
        Task<TAggregate> GetByIdAsync(TAggregateId id);

        Task SaveAsync(TAggregate aggregate);

        Task<IEnumerable<StoreEvent<TAggregateId>>> LoadAsync(TAggregateId id);
    }
}