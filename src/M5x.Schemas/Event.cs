using System;
using System.Collections.Generic;

namespace M5x.Schemas
{
    public interface IEvent<out TAggregateId> : IAggregateEvent
    {
        Guid EventId { get; }

        TAggregateId AggregateId { get; }

        long AggregateVersion { get; }
    }


    public interface IAggregateEvent : IVersionedType
    {
        public string CorrelationId { get; set; }
    }


    public abstract record Event<TAggregateId> : IEvent<TAggregateId>
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            CorrelationId = GuidFactories.NewCleanGuid;
        }

        protected Event(TAggregateId aggregateId)
        {
            EventId = Guid.NewGuid();
            AggregateId = aggregateId;
        }

        protected Event(TAggregateId aggregateId, long aggregateVersion)
        {
            EventId = Guid.NewGuid();
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }

        public Guid EventId { get; set; }

        public TAggregateId AggregateId { get; set; }

        public long AggregateVersion { get; set; }
        public string CorrelationId { get; set; }

        public override int GetHashCode()
        {
            return 240974282 + EqualityComparer<Guid>.Default.GetHashCode(EventId);
        }

        public abstract IEvent<TAggregateId> WithAggregate(TAggregateId aggregateId, long version);
    }
}