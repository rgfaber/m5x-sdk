using EventFlow.Core;

namespace M5x.CEQS.Schema
{
    public abstract record Hope<TId> : IHope<TId> 
        where TId : IIdentity
    {
        protected Hope(TId aggregateId, string correlationId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }
        
        public string CorrelationId { get; set; }
        protected Hope() {}

        public TId AggregateId { get; set; }
    }

    public interface IHope<TID> : IHope
        where TID : IIdentity
    {
        TID AggregateId { get; set; }
    }

    public interface IHope
    {
        string CorrelationId { get; set; }
        
    }
    
    
}