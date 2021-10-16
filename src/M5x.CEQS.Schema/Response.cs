using System.Collections;
using System.ComponentModel.DataAnnotations;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.CEQS.Schema
{
    public abstract record Response : IResponse
    {
        protected Response()
        {
            ErrorState = new ErrorState();
        }
        protected Response(string correlationId) : this()
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


    public interface IResponse 
    {
        [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
        [Required] ErrorState ErrorState { get; }
        bool IsSuccess { get; }
    }

    public interface IResponse<TPayload> : IResponse 
        where TPayload : IEnumerable
    {
        
    }
    

    public abstract record Response<TPayload> : Response, IResponse<TPayload> 
        where TPayload : IEnumerable
    {
        protected Response()
        {}
        
        protected Response(string correlationId) : base(correlationId)
        {
        }

        protected Response(TPayload data)
        {
            Data = data;
        }

        protected Response(string correlationId, TPayload data) : base(correlationId)
        {
            Data = data;
        }

        public TPayload Data { get; set; }
    }
}