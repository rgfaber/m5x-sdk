using M5x.Schemas;

namespace M5x.Publisher.Contract
{
    [Topic("test-raf")]
    public record TestRafReq : Request
    {
        
        
        public TestPayload Payload { get; set; }
        
        
        public TestRafReq(string aggregateId, string correlationId, string sourceId) : base(aggregateId, correlationId, sourceId)
        {
            Payload = new TestPayload();
        }

        public TestRafReq(string aggregateId, string correlationId) : base(aggregateId, correlationId)
        {
            Payload = new TestPayload();
        }

        public TestRafReq()
        {
            Payload = new TestPayload();
        }
        
        public static TestRafReq CreateNew(string aggregateId, string correlationId)
        {
            var res = new TestRafReq(aggregateId, correlationId);
            res.Payload = TestPayload.CreateRandom();
            return res;
        }
    }
    
    
    
}