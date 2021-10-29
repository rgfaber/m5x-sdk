using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC
{
    public abstract class AsyncActor<TAggregate, TAggregateId, TCommand, TFeedback>
        : IAsyncActor<TAggregateId, TCommand, TFeedback>
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregateId>
        where TFeedback : IFeedback
    {
        private readonly IDECBus _bus;
        private readonly IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> _handlers;

        protected readonly IAsyncEventStream<TAggregate, TAggregateId> Aggregates;
        protected readonly ILogger Logger;


        protected AsyncActor(
            IAsyncEventStream<TAggregate, TAggregateId> aggregates,
            IDECBus bus,
            IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
            ILogger logger)
        {
            Aggregates = aggregates;
            _bus = bus;
            _handlers = handlers;
            Logger = logger;
        }


        public async Task ReplayAsync(CancellationToken cancellationToken = default)
        {
            var events = Aggregates.LoadAllAsync(cancellationToken);
            if (events == null) return;
            var enumerator = events.GetAsyncEnumerator(cancellationToken);
            do
            {
                var ev = enumerator.Current.Event;
                await EmitAsync(_handlers, ev);
            } while (await enumerator.MoveNextAsync());
        }


        public Task<TFeedback> HandleAsync(TCommand cmd)
        {
            Guard.Against.Null(cmd, nameof(cmd));
            Guard.Against.Null(cmd.AggregateId, nameof(cmd.AggregateId));
            Guard.Against.NullOrWhiteSpace(cmd.CorrelationId, nameof(cmd.CorrelationId));
            _bus.Subscribe<IEvent<TAggregateId>>(async @event => await HandleAsync(_handlers, @event));
            return Act(cmd);
        }


        private Task EmitAsync(IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
            IEvent<TAggregateId> @event)
        {
            Guard.Against.Null(@event, nameof(@event));
            Guard.Against.Null(@event.Meta, nameof(@event.Meta));
            Guard.Against.Null(@event.EventId, nameof(@event.EventId));
            Guard.Against.NullOrWhiteSpace(@event.Meta.Id, nameof(@event.Meta.Id));
            foreach (var handler in handlers)
                handler.HandleAsync(@event);
            return Task.CompletedTask;
        }

        protected abstract Task<TFeedback> Act(TCommand cmd);

        private Task HandleAsync(IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
            IEvent<TAggregateId> @event)
        {
            foreach (var handler in handlers)
                handler.HandleAsync(@event);
            return Task.CompletedTask;
        }
    }
    
    public interface IAsyncActor {}

    public interface IAsyncActor<TAggregateId, in TCommand, TFeedback> : IAsyncActor
        where TCommand : ICommand<TAggregateId>
        where TFeedback : IFeedback
        where TAggregateId : IIdentity
    {
        Task<TFeedback> HandleAsync(TCommand cmd);
        Task ReplayAsync(CancellationToken cancellationToken = default);
    }
}