using M5x.DEC;
using M5x.DEC.Infra.STAN;
using M5x.Publisher.Contract;
using NATS.Client;
using Serilog;

namespace M5x.Stan.Subscriber.Cli
{
    public class TestRafResponder: STANResponder<RafId, TestRaf, TestRafReq,TestRafRsp>
    {
        public TestRafResponder(IEncodedConnection conn,
            ITestRafActor actor,
            ILogger logger) : base(conn,
            actor,
            logger)
        {
        }
    }
}