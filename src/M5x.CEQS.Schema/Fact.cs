using EventFlow.Core;

namespace M5x.CEQS.Schema
{

    public interface IFact
    {
        public string CorrelationId { get; set; }        
    }

    public interface IFact<TAggregateId>: IFact
    {
        TAggregateId AggregateId { get; set; }
    }
    
    
    public interface IFact<TAggregateId, TPayload>: IFact<TAggregateId>  
        where TAggregateId: IIdentity
        where TPayload: class
    {
        TPayload Payload { get; set; }
    }
    
    
    public abstract record Fact<TAggregateId> : IFact<TAggregateId>
    {
        protected Fact(TAggregateId aggregateId, string correlationId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }

        public TAggregateId AggregateId { get; set; }
        public string CorrelationId { get; set; }
    }


    public abstract record Fact<TAggregateId, TPayload> :  Fact<TAggregateId>, IFact<TAggregateId, TPayload> 
        where TPayload : class 
        where TAggregateId : IIdentity
    {
       
        public TPayload Payload { get; set; }

        protected Fact(TAggregateId aggregateId, string correlationId) : base(aggregateId, correlationId)
        {
        }
    }
}