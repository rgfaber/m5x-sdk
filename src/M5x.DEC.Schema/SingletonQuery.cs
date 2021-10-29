namespace M5x.DEC.Schema
{
    public abstract record SingletonQuery : Query, ISingletonQuery
    {
        protected SingletonQuery()
        {
        }

        protected SingletonQuery(string id,string correlationId) : base(correlationId)
        {
            Id = id;
            CorrelationId = correlationId;
        }

        public string Id { get; set; }
    }

    public interface ISingletonQuery : IQuery
    {
        string Id { get; set; }
    }
}