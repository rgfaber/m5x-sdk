using System.Collections.Generic;
using M5x.DEC;
using M5x.DEC.Infra.STAN;
using M5x.Publisher.Contract;
using NATS.Client;
using Serilog;

namespace M5x.Stan.Subscriber.Cli
{
    public class RafTestedListener: STANListener<RafId, RafTested>
    {
        public RafTestedListener(IEncodedConnection conn,
            IEnumerable<IRafTestedHandler> handlers,
            ILogger logger) : base(conn,
            handlers,
            logger)
        {
        }
    }


    public interface IRafTestedHandler: IAggregateEventHandler<RafId, RafTested> {}
}