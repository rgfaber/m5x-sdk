using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC
{
    public abstract class
        AsyncActor<TAggregate, TAggregateId, TCommand, THope, TFeedback, TEvent, TFact>
        : IAsyncActor<TAggregate, TAggregateId, TCommand, THope, TFeedback>
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregate, TAggregateId, IExecutionResult>
        where TFeedback : IFeedback
        where TFact : IFact
        where TEvent : IEvent<TAggregateId>
        where THope : IHope

    {
        private readonly IFactEmitter<TAggregateId, TFact> _emitter;
        private readonly IEnumerable<IEventHandler<TAggregateId, TEvent>> _handlers;
        private readonly IDECBus _bus;

        protected readonly IAsyncEventStream<TAggregate, TAggregateId> Aggregates;
        protected readonly ILogger Logger;


        protected AsyncActor(
            IAsyncEventStream<TAggregate, TAggregateId> aggregates,
            IDECBus bus,
            IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers,
            IFactEmitter<TAggregateId, TFact> emitter,
            ILogger logger)
        {
            Aggregates = aggregates;
            _bus = bus;
            _handlers = handlers;
            _emitter = emitter;
            Logger = logger;
        }


        public async Task ReplayAsync(CancellationToken cancellationToken=default)
        {
            var events = Aggregates.LoadAllAsync(cancellationToken);
            if (events == null) return;
            var enumerator = events.GetAsyncEnumerator(cancellationToken);
            do
            {
                var ev = (TEvent)enumerator.Current.Event;
                await EmitAsync(_handlers, ev);
            } while (await enumerator.MoveNextAsync());
        }
        
        
        private Task EmitAsync(IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers,
            TEvent @event)
        {
            Guard.Against.Null(@event, nameof(@event));
            Guard.Against.Null(@event.Meta, nameof(@event.Meta));
            Guard.Against.Null(@event.EventId, nameof(@event.EventId));
            Guard.Against.NullOrWhiteSpace(@event.Meta.Id, nameof(@event.Meta.Id));
            
            foreach (var handler in handlers)
                handler.HandleAsync(@event);
            
            return _emitter?.EmitAsync(ToFact(@event));
        }

        
        

        public async Task<TFeedback> HandleAsync(THope hope)
        {
            Guard.Against.Null(hope, nameof(hope));
            Guard.Against.NullOrWhiteSpace(hope.AggregateId, nameof(hope.AggregateId));
            
            _bus.Subscribe<TEvent>(async @event => await HandleAsync(_handlers, @event));
            
            var feedback = await Act(ToCommand(hope));
            
            return feedback;
        }

        protected abstract TCommand ToCommand(THope hope);


        protected abstract Task<TFeedback> Act(TCommand cmd);


        private async Task HandleAsync(IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers,
            TEvent @event)
        {
            await _emitter.EmitAsync(ToFact(@event));
            foreach (var handler in handlers)
                await handler.HandleAsync(@event);
        }

        protected abstract TFact ToFact(TEvent @event);
    }

    public interface IAsyncActor<TAggregate, in TAggregateId, TCommand, THope, TFeedback>
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where THope : IHope
        where TCommand : ICommand<TAggregate, TAggregateId, IExecutionResult>
    {
        Task<TFeedback> HandleAsync(THope hope);
        Task ReplayAsync(CancellationToken cancellationToken = default);
    }
}