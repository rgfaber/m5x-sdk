namespace M5x.Schemas
{
    public abstract record Query : Request
    {
        protected Query(string correlationId) : this()
        {
            CorrelationId = correlationId;
        }

        protected Query()
        {
            CorrelationId = GuidFactories.NewCleanGuid;
        }
    }


    public abstract record Query<T> : Query where T : new()
    {
        protected Query()
        {
            Data = new T();
        }

        public Query(string correlationId) : base(correlationId)
        {
            Data = new T();
        }

        public T Data { get; set; }
    }
}