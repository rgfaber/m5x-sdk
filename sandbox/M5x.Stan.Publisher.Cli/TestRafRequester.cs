using M5x.DEC;
using M5x.DEC.Infra.STAN;
using M5x.Publisher.Contract;
using NATS.Client;
using Serilog;

namespace M5x.Stan.Publisher.Cli
{
    internal class TestRafRequester : STANRequester<TestRafReq, TestRafRsp>, ITestRafRequester 
    {
        public TestRafRequester(IEncodedConnection conn, ILogger logger) : base(conn, logger)
        {
        }
    }

    public interface ITestRafRequester: IRequester<TestRafReq, TestRafRsp>
    {
    }
}