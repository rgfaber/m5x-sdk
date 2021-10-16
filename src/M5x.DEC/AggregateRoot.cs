using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public abstract class AggregateRoot<TId, TState, TStatus> : AggregateRoot<TId, TState>
        where TId : IIdentity
        where TState : IStateEntity<TId, TStatus>
        where TStatus : Enum
    {
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TId id, TState model) : base(id, model)
        {
        }

        protected AggregateRoot(TState model) : base(model)
        {
        }

        public TStatus Status => Model != null ? Model.Status : default;
    }


    public abstract class AggregateRoot<TId, TState> : AggregateRoot<TId>
        where TId : IIdentity
        where TState : IStateEntity<TId>
    {
        protected AggregateRoot(TId id, TState model) : base(id)
        {
            Model = model;
        }

        protected AggregateRoot(TState model)
        {
            Model = model;
        }

        protected AggregateRoot()
        {
            Model = CreateModel();
        }

        protected AggregateRoot(TId id) : base(id)
        {
        }

        protected TState Model { get; set; }

        protected abstract TState CreateModel();
    }


    public abstract class AggregateRoot<TId> : IEventSourcingAggregate<TId>
        where TId : IIdentity
    {
        public const long NewAggregateVersion = -1;

        private readonly ICollection<IEvent<TId>> _uncommittedEvents = new LinkedList<IEvent<TId>>();

        private ISourceID _sourceId;

        protected AggregateRoot(TId id)
        {
            Id = id;
        }

        protected AggregateRoot()
        {
        }

        [Required] public TId Id { get; set; }

        public IAggregateName Name { get; }

        public bool IsNew { get; }

        public bool HasSourceId(ISourceID sourceId)
        {
            return _sourceId == sourceId;
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public long Version { get; private set; } = NewAggregateVersion;


        void IEventSourcingAggregate<TId>.ApplyEvent(IEvent<TId> @event, long version)
        {
            if (_uncommittedEvents.Any(x => Equals(x.EventId, @event.EventId))) return;
            ((dynamic)this).Apply((dynamic)@event);
            Version = version;
        }

        /// <summary>
        /// This overload should allow events of type TEvent<TID,TPayload>
        /// </summary>
        /// <param name="event"></param>
        /// <param name="version"></param>
        /// <typeparam name="TPayload"></typeparam>
        // void IEventSourcingAggregate<TId>.ApplyEvent<TPayload>(IEvent<TId, TPayload> @event, long version)
        // {
        //     if (_uncommittedEvents.Any(x => Equals(x.EventId, @event.EventId))) return;
        //     ((dynamic)this).Apply((dynamic)@event);
        //     Version = version;
        // }

        
        
        // void IEventSourcingAggregate<TId>.ApplyEvent<TPayload>(IEvent<TId, TPayload> @event, long version)
        // {
        //     if (_uncommittedEvents.Any(x => Equals(x.EventId, @event.EventId))) return;
        //     ((dynamic) this).Apply((dynamic) @event);
        //     _version = version;
        // }


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
            foreach (var @event in aggregateEvents)
                _uncommittedEvents.Add(@event);
        }

        public static TId FromString(string value)
        {
            try
            {
                return (TId)Activator.CreateInstance(typeof(TId), value);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null) throw e.InnerException;
                throw;
            }
        }

        protected void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : Event<TId>
        {
            var meta = Equals(Id, default(TId))
                ? @event.Meta
                : AggregateInfo.New(Id.Value, Version, @event.Meta.Status);

            var eventWithAggregate = @event.WithAggregate(meta, @event.CorrelationId);

            eventWithAggregate.CorrelationId = @event.CorrelationId;

            ((IEventSourcingAggregate<TId>)this).ApplyEvent(eventWithAggregate, Version++);

            _uncommittedEvents.Add(eventWithAggregate);
        }


        public bool Validate(params Func<IAggregateRoot, bool>[] validations)
        {
            return validations.Aggregate(
                true,
                (current,
                    validation) => current && validation.Invoke(this));
        }
    }
}