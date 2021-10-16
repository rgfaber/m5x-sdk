using System.Collections;

namespace M5x.CEQS.Schema
{
    public abstract record PagedResponse<TPayload> : Response<TPayload> 
        where TPayload : class, IEnumerable
    {
        protected PagedResponse(int pageNumber)
        {
            PageNumber = pageNumber;
        }

        protected PagedResponse(string correlationId, int pageNumber) : base(correlationId)
        {
            PageNumber = pageNumber;
        }
        public int PageNumber { get; set; }
    }
}