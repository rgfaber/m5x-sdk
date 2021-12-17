using MyNatsClient;

namespace M5x.Nats.Interfaces;

public interface IBusBuilder
{
    INatsClient BuildBus(ConnectionInfo conn = null, ISocketFactory sockFact = null,
        IConsumerFactory consFact = null);
}