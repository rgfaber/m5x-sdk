namespace M5x.DEC.Schema
{
    
    public interface ISingletonResponse<TPayload> : IResponse
        where TPayload : IPayload
    {
        TPayload Payload { get; set; }
    }

    
    public abstract record SingletonResponse<TPayload> : Response, ISingletonResponse<TPayload>
        where TPayload : IPayload
    {
        protected SingletonResponse()
        {
        }

        protected SingletonResponse(string correlationId) : base(correlationId)
        {
        }

        protected SingletonResponse(TPayload payload)
        {
            Payload = payload;
        }

        protected SingletonResponse(string correlationId, TPayload payload) : base(correlationId)
        {
            Payload = payload;
        }

        public TPayload Payload { get; set; }
    }
}