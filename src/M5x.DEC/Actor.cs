using System.Collections.Generic;
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
        Actor<TAggregate, TAggregateId, TCommand, THope, TFeedback, TEvent, TFact>
        : IActor<TAggregate, TAggregateId, TCommand, THope, TFeedback>
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregate, TAggregateId, IExecutionResult>
        where TFeedback : IFeedback
        where TFact : IFact
        where TEvent : IEvent<TAggregateId>
        where THope : IHope

    {
        private readonly IDECBus _bus;
        private readonly IFactEmitter<TAggregateId, TFact> _emitter;
        private readonly IEnumerable<IEventHandler<TAggregateId, TEvent>> _handlers;

        protected readonly IEventStream<TAggregate, TAggregateId> Aggregates;
        protected readonly ILogger Logger;

        protected Actor(
            IEventStream<TAggregate, TAggregateId> aggregates,
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


        public void Replay(TAggregateId id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(id.Value, nameof(id.Value));

            var events = Aggregates.Load(id);
            foreach (var @event in events)
            {
                var ev = (TEvent)@event.Event;
                Emit(_handlers, ev);
            }
        }

        public TFeedback Handle(THope hope)
        {
            Guard.Against.Null(hope, nameof(hope));
            Guard.Against.NullOrWhiteSpace(hope.AggregateId, nameof(hope.AggregateId));

            _bus.Subscribe<TEvent>(@event => Emit(_handlers, @event));
            return Act(ToCommand(hope));
        }

        protected abstract TCommand ToCommand(THope hope);
        protected abstract TFeedback Act(TCommand cmd);

        private void Emit(IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers,
            TEvent @event)
        {
            Guard.Against.Null(@event, nameof(@event));
            Guard.Against.Null(@event.Meta, nameof(@event.Meta));
            Guard.Against.Null(@event.EventId, nameof(@event.EventId));
            Guard.Against.NullOrWhiteSpace(@event.Meta.Id, nameof(@event.Meta.Id));
            
            _emitter?
                .EmitAsync(ToFact(@event)).Wait();
            foreach (var handler in handlers)
                handler.HandleAsync(@event).Wait();
        }

        protected abstract TFact ToFact(TEvent @event);
    }

    public interface IActor<TAggregate, in TAggregateId, TCommand, THope, TFeedback>
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where THope : IHope
        where TCommand : ICommand<TAggregate, TAggregateId, IExecutionResult>
    {
        TFeedback Handle(THope hope);
        void Replay(TAggregateId id);
    }
}