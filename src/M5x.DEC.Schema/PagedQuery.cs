namespace M5x.DEC.Schema
{
    public abstract record PagedQuery : MultiQuery
    {
        protected PagedQuery()
        {
        }

        protected PagedQuery(string correlationId, int pageNumber, int pageSize) : base(correlationId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        protected PagedQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }


        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}