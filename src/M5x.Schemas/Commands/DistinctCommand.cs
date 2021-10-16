using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace M5x.Schemas.Commands
{
    public abstract class DistinctCommand<TIdentity> : ICommand<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly Lazy<ISourceID> _lazySourceId;

        protected DistinctCommand(
            TIdentity aggregateId)
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            _lazySourceId = new Lazy<ISourceID>(CalculateSourceId, LazyThreadSafetyMode.PublicationOnly);

            AggregateId = aggregateId;
        }

        protected DistinctCommand(TIdentity aggregateId, string correlationId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }

        public ISourceID SourceId => _lazySourceId.Value;
        public TIdentity AggregateId { get; }


        public ISourceID GetSourceId()
        {
            return SourceId;
        }

        public string CorrelationId { get; set; }

        private CommandId CalculateSourceId()
        {
            var bytes = GetSourceIdComponents().SelectMany(b => b).ToArray();
            return CommandId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Commands,
                bytes);
        }

        protected abstract IEnumerable<byte[]> GetSourceIdComponents();
    }
}