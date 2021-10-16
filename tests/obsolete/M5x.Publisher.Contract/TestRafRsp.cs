using M5x.Schemas;

namespace M5x.Publisher.Contract
{
    public record TestRafRsp : Response
    {
        public TestRafRsp()
        {
        }

        public TestRafRsp(string correlationId) : base(correlationId)
        {
        }
    }
}