using System;
using M5x.DEC.Schema;

namespace M5x.DEC.Commands
{
    // public abstract record Command<TAggregate, TAggregateId> :
    //     Command<TAggregateId>,
    //     ICommand<TAggregate, TAggregateId>
    //     where TAggregate : IAggregateRoot<TAggregateId>
    //     where TAggregateId : IIdentity
    // {
    //     protected Command(TAggregateId aggregateId) : base(aggregateId)
    //     {
    //     }
    //
    //     protected Command(TAggregateId aggregateId, CommandId sourceId) : base(aggregateId, sourceId)
    //     {
    //     }
    //
    //     protected Command()
    //     {
    //     }
    //
    //     protected Command(TAggregateId aggregateId, string correlationId) : base(aggregateId, correlationId)
    //     {
    //     }
    // }


    public abstract record Command<TIdentity, TSourceIdentity> :
        ICommand<TIdentity, TSourceIdentity>
        where TIdentity : IIdentity
        where TSourceIdentity : ISourceID
    {
        protected Command(
            TIdentity aggregateId,
            TSourceIdentity sourceId)
        {
            if (aggregateId == null) throw new ArgumentNullException(nameof(aggregateId));
            if (sourceId == null) throw new ArgumentNullException(nameof(sourceId));
            AggregateId = aggregateId;
            SourceId = sourceId;
        }

        protected Command(TSourceIdentity sourceId, TIdentity aggregateAggregateId)
        {
            SourceId = sourceId;
            AggregateId = aggregateAggregateId;
        }

        protected Command(string correlationId)
        {
            CorrelationId = correlationId;
        }

        protected Command(TSourceIdentity sourceId, TIdentity aggregateAggregateId, string correlationId)
        {
            SourceId = sourceId;
            AggregateId = aggregateAggregateId;
            CorrelationId = correlationId;
        }

        protected Command()
        {
        }

        protected Command(TIdentity aggregateId, string correlationId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }

        public TSourceIdentity SourceId { get; }
        public TIdentity AggregateId { get; }

        public ISourceID GetSourceId()
        {
            return SourceId;
        }

        public string CorrelationId { get; set; }
    }


    public abstract record Command<TID> :
        Command<TID, ISourceID>, ICommand<TID>
        where TID : IIdentity
    {
        protected Command(
            TID aggregateId)
            : this(aggregateId, CommandId.New)
        {
        }

        protected Command(
            TID aggregateId,
            CommandId sourceId)
            : base(aggregateId, sourceId)
        {
        }

        protected Command(TID aggregateId, ISourceID sourceId) : base(aggregateId, sourceId)
        {
        }

        protected Command(ISourceID sourceId, TID Id) : base(sourceId, Id)
        {
        }

        protected Command(string correlationId) : base(correlationId)
        {
        }

        protected Command(ISourceID sourceId, TID Id, string correlationId) : base(sourceId, Id, correlationId)
        {
        }

        protected Command(TID aggregateId, string correlationId) : base(aggregateId, correlationId)
        {
        }


        protected Command()
        {
        }
    }
}