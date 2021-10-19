using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using M5x.DEC.Schema.Utils;

namespace M5x.DEC.Schema
{
    public interface IFeedback
    {
        [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
        [Required] ErrorState ErrorState { get; }
        [Required] AggregateInfo Meta { get; set; }

        bool IsSuccess { get; }
    }

    public interface IFeedback<TPayload> : IFeedback
        where TPayload : IPayload
    {
        TPayload Payload { get; set; }
    }

    public abstract record Feedback : IFeedback
    {
        protected Feedback(AggregateInfo meta, string correlationId)
        {
            CorrelationId = correlationId;
            Meta = meta;
            ErrorState = new ErrorState();
        }

        protected Feedback()
        {
            Meta = AggregateInfo.Empty;
            CorrelationId = GuidUtils.NewCleanGuid;
            ErrorState = new ErrorState();
        }

        public string CorrelationId { get; set; }
        public ErrorState ErrorState { get; }
        public AggregateInfo Meta { get; set; }
        public bool IsSuccess => ErrorState.IsSuccessful;
    }
    
    

    public abstract record Feedback<TPayload> : Feedback, IFeedback<TPayload> where TPayload : IPayload
    {
        protected Feedback()
        {
            Meta = AggregateInfo.Empty;
            CorrelationId = GuidUtils.NewCleanGuid;
        }

        protected Feedback(AggregateInfo meta, string correlationId, TPayload payload)
        {
            Meta = meta;
            CorrelationId = correlationId;
            Payload = payload;
        }

        public TPayload Payload { get; set; }

        public override string ToString()
        {
            return $"Feedback: {JsonSerializer.Serialize(this)}";
        }
    }
}