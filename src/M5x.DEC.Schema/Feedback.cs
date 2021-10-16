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

    public abstract record Feedback<TPayload> : IFeedback<TPayload> where TPayload : IPayload
    {
        protected Feedback()
        {
            Meta = AggregateInfo.Empty;
            CorrelationId = GuidUtils.NewCleanGuid;
            ErrorState = new ErrorState();
        }

        protected Feedback(AggregateInfo meta, string correlationId, TPayload payload)
        {
            ErrorState = new ErrorState();
            Meta = meta;
            CorrelationId = correlationId;
            Payload = payload;
        }

        protected Feedback(string correlationId)
        {
            ErrorState = new ErrorState();
            CorrelationId = correlationId;
            Meta = AggregateInfo.Empty;
        }


        public bool IsSuccess => ErrorState.IsSuccessful;
        [Required] public ErrorState ErrorState { get; }
        [Required] public AggregateInfo Meta { get; set; }
        [Required(AllowEmptyStrings = false)] public string CorrelationId { get; set; }
        public TPayload Payload { get; set; }

        public override string ToString()
        {
            return $"Feedback: {JsonSerializer.Serialize(this)}";
        }
    }
}