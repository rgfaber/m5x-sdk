using System;

namespace M5x.Schemas.Commands
{

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

        protected Command(TSourceIdentity sourceId, TIdentity aggregateId)
        {
            SourceId = sourceId;
            AggregateId = aggregateId;
        }

        protected Command(string correlationId)
        {
            CorrelationId = correlationId;
        }

        protected Command(TSourceIdentity sourceId, TIdentity aggregateId, string correlationId)
        {
            SourceId = sourceId;
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }

        protected Command()
        {
        }

        public TSourceIdentity SourceId { get; }
        public TIdentity AggregateId { get; }

        public ISourceID GetSourceId()
        {
            return SourceId;
        }

        public string CorrelationId { get; set; }
    }


    public abstract record Command<TIdentity> :
        Command<TIdentity, ISourceID>
        where TIdentity : IIdentity
    {
        protected Command(
            TIdentity aggregateId)
            : this(aggregateId, CommandId.New)
        {
        }

        protected Command(
            TIdentity aggregateId,
            CommandId sourceId)
            : base(aggregateId, sourceId)
        {
        }

        protected Command() 
        {
        }
    }
}