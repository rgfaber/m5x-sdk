using System.Collections.Generic;

namespace M5x.DEC.Schema
{
    public interface IMultiResponse<TPayload> : IResponse
        where TPayload : IPayload
    {
        IEnumerable<TPayload> Payload { get; set; }
    }

    public abstract record MultiResponse<TPayload> : Response, IMultiResponse<TPayload>
        where TPayload : IPayload
    {
        protected MultiResponse(string correlationId, IEnumerable<TPayload> payload) : base(correlationId)
        {
            Payload = payload;
        }

        protected MultiResponse()
        {
        }

        protected MultiResponse(IEnumerable<TPayload> payload)
        {
            Payload = payload;
        }

        public IEnumerable<TPayload> Payload { get; set; }
    }
}