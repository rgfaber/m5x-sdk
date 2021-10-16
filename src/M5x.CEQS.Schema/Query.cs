namespace M5x.CEQS.Schema
{
    public abstract record Query : IQuery 
    {
        protected Query()
        {
        }

        protected Query(string correlationId) 
        { }

        public string CorrelationId { get; set; }
    }

    public interface IQuery
    {
        public string CorrelationId { get; set; }
    }

}