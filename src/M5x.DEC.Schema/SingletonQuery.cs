namespace M5x.DEC.Schema
{
    public abstract record SingletonQuery : Query, ISingletonQuery
    {
        protected SingletonQuery() {}
        protected SingletonQuery(string id)
        {
            Id = id;
        }

        protected SingletonQuery(string correlationId, string id) : base(correlationId)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
    
    public interface ISingletonQuery: IQuery
    {
        string Id { get; set; }
    }

}