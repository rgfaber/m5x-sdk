using System.ComponentModel.DataAnnotations;

namespace M5x.Schemas
{
    public abstract record Request : IRequest
    {
        protected Request(string aggregateId, string correlationId, string sourceId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            SourceId = sourceId;
        }

        protected Request(string aggregateId, string correlationId)
        {
            AggregateId = aggregateId;
            CorrelationId = correlationId;
        }

        [Required]
        public string AggregateId { get; set; }

        public Request()
        { }


        public string CorrelationId { get; set; }
        public string SourceId { get; set; }
    }

    public interface IRequest : IVersionedType, ICorrelated
    {
    }
}