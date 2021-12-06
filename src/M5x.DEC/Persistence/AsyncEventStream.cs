using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence
{
    public interface IAsyncEventStream
    {
    }

    public interface IAsyncEventStream<TAggregate, TAggregateId> : IAsyncEventStream
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
    {
        Task<TAggregate> GetByIdAsync(TAggregateId id);
        Task SaveAsync(TAggregate aggregate);
        Task<IEnumerable<StoreEvent<TAggregateId>>> LoadAsync(TAggregateId id);
        IAsyncEnumerable<StoreEvent<TAggregateId>> LoadAllAsync(CancellationToken cancellationToken = default);
    }


    public abstract class AsyncEventStream<TAggregate, TAggregateId> : IAsyncEventStream<TAggregate, TAggregateId>
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
    {
        private readonly IDECBus bus;
        private readonly IEventStore eventStore;

        public AsyncEventStream(IEventStore eventStore, IDECBus bus)
        {
            this.eventStore = eventStore;
            this.bus = bus;
        }


        public async Task<TAggregate> GetByIdAsync(TAggregateId id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate(id);
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;
                var lst = await eventStore.ReadEventsAsync(id);
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

        public IAsyncEnumerable<StoreEvent<TAggregateId>> LoadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return eventStore.ReadAllEventsAsync<TAggregateId>(cancellationToken);
            }
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
                    await eventStore.AppendEventAsync(@event).ConfigureAwait(false);
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
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(id.Value, nameof(id.Value));
            try
            {
                var ci = typeof(TAggregate)
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                        null,
                        new [] {typeof(TAggregateId)} ,
                        Array.Empty<ParameterModifier>());
                Guard.Against.Null(ci, nameof(ci));
                var res = ci.Invoke(new object[] {id});
                var result = (TAggregate)res;
//                result.Id = id;
                return result;
            }
            catch (Exception e)
            {
                throw new RepositoryException("Failed at Creating an Empty Aggregate!", e);
            }
        }
    }
}