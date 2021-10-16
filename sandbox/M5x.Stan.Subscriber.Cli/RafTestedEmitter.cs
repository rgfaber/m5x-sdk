using M5x.DEC;
using M5x.DEC.Infra.STAN;
using M5x.Publisher.Contract;
using NATS.Client;
using Serilog;

namespace M5x.Stan.Subscriber.Cli
{
    public class RafTestedEmitter: STANEmitter<RafId, RafTested>, IRafTestedEmitter
    {
        public RafTestedEmitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
        {
        }
    }
    
    
    public interface IRafTestedEmitter : IEventEmitter<RafId, RafTested>
    {
    }

    
}