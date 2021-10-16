using M5x.Nats.Interfaces;
using MyNatsClient;

namespace M5x.Nats
{
    public class BusBuilder : IBusBuilder
    {
        public INatsClient BuildBus(ConnectionInfo conn = null, ISocketFactory sockFact = null,
            IConsumerFactory consFact = null)
        {
            if (conn != null) return new NatsClient(conn, sockFact, consFact);
            conn = new ConnectionInfo(BusConfig.Host, BusConfig.Port);
            if (!string.IsNullOrWhiteSpace(BusConfig.User))
                conn.Credentials = new Credentials(BusConfig.User, BusConfig.Password);
            return new NatsClient(conn, sockFact, consFact);
        }
    }
}