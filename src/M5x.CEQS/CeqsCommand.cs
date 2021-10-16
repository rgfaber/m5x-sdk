using System;
using System.Collections.Generic;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{

    public interface ICeqsCommand<TIdentity> where TIdentity : IIdentity
    {
        Hope<TIdentity> Hope { get; set; }
    }
    
    
    public abstract class CeqsCommand<TAggregate,TIdentity,TResult> : Command<TAggregate, TIdentity, TResult>, ICeqsCommand<TIdentity> 
        where TAggregate: IAggregateRoot<TIdentity>
        where TIdentity: IIdentity
        where TResult : IFeedback
    {
        
        protected CeqsCommand(TIdentity aggregateId) : base(aggregateId)
        {
        }

        protected CeqsCommand(TIdentity aggregateId, ISourceId sourceId) : base(aggregateId, sourceId)
        {
        }

        protected CeqsCommand(TIdentity aggregateId, Hope<TIdentity> hope) : base(aggregateId)
        {
            Hope = hope;
        }

        protected CeqsCommand(TIdentity aggregateId, ISourceId sourceId, Hope<TIdentity> hope) : base(aggregateId, sourceId)
        {
            Hope = hope;
        }

        protected CeqsCommand(TIdentity aggregateId, Hope<TIdentity> hope, string correlationId) : base(aggregateId)
        {
            Hope = hope;
            CorrelationId = correlationId;
        }

        protected CeqsCommand(TIdentity aggregateId, ISourceId sourceId, Hope<TIdentity> hope, string correlationId) : base(aggregateId, sourceId)
        {
            Hope = hope;
            CorrelationId = correlationId;
        }
        public Hope<TIdentity> Hope { get; set; }
        public string CorrelationId { get; set; }


        public override int GetHashCode()
        {
            return 240974282 + EqualityComparer<Guid>.Default.GetHashCode(Guid.Parse(AggregateId.Value)) +base.GetHashCode() + Hope.GetHashCode();
        }
    }
}