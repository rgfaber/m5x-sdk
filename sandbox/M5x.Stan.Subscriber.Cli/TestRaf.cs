using M5x.DEC.Commands;
using M5x.Publisher.Contract;
using M5x.Schemas;


namespace M5x.Stan.Subscriber.Cli
{
    [Topic("test-raf")]
    public record TestRaf : PayloadCommand<RafRoot, RafId, TestRafReq>
    {
        public TestRaf(RafId aggregateId, TestRafReq payload) : base(aggregateId, payload)
        {
        }

        public TestRaf(RafId aggregateId) : base(aggregateId)
        {
        }


        public static TestRaf CreateNew(RafId id, TestRafReq req)
        {
            return new TestRaf(id, req);
            ;
        }
    }
}