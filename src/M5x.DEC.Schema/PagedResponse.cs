using System.Collections.Generic;

namespace M5x.DEC.Schema
{
    public interface IPagedResponse<TPayload> : IMultiResponse<TPayload>
        where TPayload : IPayload
    {
        int PageNumber { get; set; }
    }

    public abstract record PagedResponse<TPayload> : MultiResponse<TPayload>, IPagedResponse<TPayload>
        where TPayload : IPayload
    {
        protected PagedResponse()
        {
        }

        protected PagedResponse(int pageNumber)
        {
            PageNumber = pageNumber;
        }


        protected PagedResponse(int pageNumber, IEnumerable<TPayload> payload) : base(payload)
        {
            PageNumber = pageNumber;
        }

        protected PagedResponse(string correlationId, int pageNumber, IEnumerable<TPayload> payload) : base(
            correlationId, payload)
        {
            PageNumber = pageNumber;
        }

        public int PageNumber { get; set; }
    }
}