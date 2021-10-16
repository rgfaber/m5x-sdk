using System.ComponentModel.DataAnnotations;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Core;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.CEQS.Schema
{
    public abstract record Feedback : IFeedback
    {
        public Feedback()
        {
            ErrorState = new ErrorState();
        }
        protected Feedback(string correlationId) : this()
        {
            CorrelationId = correlationId;
        }
        [Required] public ErrorState ErrorState { get; }
        
        public int AggregateStatus { get; set; }
        [Required(AllowEmptyStrings = false)] public string CorrelationId { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
        public bool IsSuccess => ErrorState.IsSuccessful;
    }


    public interface IFeedback : IExecutionResult
    {
        [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
        [Required] ErrorState ErrorState { get; }
        [Required] int AggregateStatus { get; set; }
    }

    public interface IFeedback<TAggregateId> : IFeedback
    where TAggregateId : IIdentity
    {
        [Required]TAggregateId AggregateId { get; set; }
    }

    public abstract record Feedback<TAggregateId> : Feedback, IFeedback<TAggregateId>
        where TAggregateId : IIdentity, new()
    {
       
        protected Feedback(TAggregateId aggregateId, string correlationId) : base(correlationId)
        {
            AggregateId = aggregateId;
        }

        protected Feedback(TAggregateId aggregateId)
        {
            AggregateId = aggregateId;
        }

        protected Feedback()
        {
            AggregateId = new TAggregateId();
        }

        public TAggregateId AggregateId { get; set; }
    }



}