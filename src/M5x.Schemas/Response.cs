using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace M5x.Schemas
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
        [Required(AllowEmptyStrings = false)] public string CorrelationId { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public interface IResponse
    {
        [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
        [Required] ErrorState ErrorState { get; }
    }


    public abstract record Response<T> : Response
    {
        protected Response()
        {
        }

        protected Response(string correlationId) : base(correlationId)
        {
        }

        public T Data { get; set; }
    }
}