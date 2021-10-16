using System;
using System.Collections.Generic;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Events
{
    // public interface IEvent<out TAggregateId, TPayload> : IEvent<TAggregateId>
    //     where TAggregateId : IIdentity
    //     where TPayload : IPayload
    //
    // {
    //     TPayload Payload { get; set; }
    // }

    public interface IEvent<out TAggregateId> : IAggregateEvent
        where TAggregateId : IIdentity
    {
        // public string CorrelationId { get; set; }
        //
        // Guid EventId {get; }
        //
        // AggregateInfo Meta { get; }
    }


    public interface IAggregateEvent : IVersionedType
    {
        public string CorrelationId { get; set; }

        Guid EventId { get; }

        AggregateInfo Meta { get; }
    }


    // public abstract record Event<TAggregateId, TPayload> : Event<TAggregateId>,
    //     IEvent<TAggregateId, TPayload> where TAggregateId : IIdentity
    //     where TPayload : IPayload
    // {
    //     protected Event(TPayload payload)
    //     {
    //         Payload = payload;
    //     }
    //
    //     protected Event(AggregateInfo meta, TPayload payload) : base(meta)
    //     {
    //         Payload = payload;
    //     }
    //
    //     protected Event(AggregateInfo meta, string correlationId, TPayload payload) : base(meta, correlationId)
    //     {
    //         Payload = payload;
    //     }
    //
    //     protected Event()
    //     {
    //     }
    //
    //     public TPayload Payload { get; set; }
    //
    //     public override IEvent<TAggregateId> WithAggregate(AggregateInfo meta, string correlationId)
    //     {
    //         return new Event<TAggregateId, TPayload>(meta, correlationId, Payload);
    //     }
    // }


    public abstract record Event<TAggregateId> : IEvent<TAggregateId>
        where TAggregateId : IIdentity
    {
        protected Event()
        {
            EventId = Guid.NewGuid();
            CorrelationId = GuidFactories.NewCleanGuid;
        }

        protected Event(AggregateInfo meta, string correlationId)
        {
            EventId = Guid.NewGuid();
            Meta = meta;
            CorrelationId = correlationId;
        }

        protected Event(AggregateInfo meta)
        {
            EventId = Guid.NewGuid();
            Meta = meta;
        }


        public Guid EventId { get; set; }
        public AggregateInfo Meta { get; set; }

        // public TAggregateId AggregateId { get; set; }
        // public long AggregateVersion { get; set; }
        public string CorrelationId { get; set; }

        public override int GetHashCode()
        {
            return 240974282 + EqualityComparer<Guid>.Default.GetHashCode(EventId);
        }

//        public abstract IEvent<TAggregateId> WithAggregate(TAggregateId aggregateId, long version);
        public abstract IEvent<TAggregateId> WithAggregate(AggregateInfo meta, string correlationId);
    }
}