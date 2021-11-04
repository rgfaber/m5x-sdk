using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;


namespace M5x.DEC
{
    [Obsolete("Please use AsyncActor instead")]
    public abstract class
        Actor<TAggregate, TAggregateId, TCommand, TFeedback>
        : IActor<TAggregateId, TCommand, TFeedback>
        where TAggregate : IEventSourcingAggregate<TAggregateId>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregateId>
        where TFeedback : IFeedback
    {
        private readonly IDECBus _bus;

        private readonly IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> _handlers;

        private readonly IBroadcaster<TAggregateId> _caster;
        protected readonly IEventStream<TAggregate, TAggregateId> Aggregates;

        protected Actor(
            IBroadcaster<TAggregateId> caster,
            IEventStream<TAggregate, TAggregateId> aggregates,
            IDECBus bus,
            IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers)
        {
            _caster = caster;
            Aggregates = aggregates;
            _bus = bus;
            _handlers = handlers;
        }


        public TFeedback Handle(TCommand cmd)
        {
            Guard.Against.Null(cmd, nameof(cmd));
            Guard.Against.Null(cmd.AggregateId, nameof(cmd.AggregateId));
            _bus.Subscribe<IEvent<TAggregateId>>(@event => Emit(_handlers, @event));
            return Act(cmd);
        }

        protected abstract TFeedback Act(TCommand cmd);

        private void Emit(IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
            IEvent<TAggregateId> @event)
        {
            Guard.Against.Null(@event, nameof(@event));
            Guard.Against.Null(@event.Meta, nameof(@event.Meta));
            Guard.Against.Null(@event.EventId, nameof(@event.EventId));
            Guard.Against.NullOrWhiteSpace(@event.Meta.Id, nameof(@event.Meta.Id));
            foreach (var handler in handlers)
                handler.HandleAsync(@event).Wait();
        }
    }

    public interface IActor<in TAggregateId, in TCommand, out TFeedback>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregateId>
        where TFeedback : IFeedback
    {
        TFeedback Handle(TCommand cmd);
    }
    
    
}