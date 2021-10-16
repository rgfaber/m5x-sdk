using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using Microsoft.Extensions.Hosting;

namespace M5x.DEC.Infra.RabbitMq
{
    public class RmqSubscriber<TAggregateId, TFact>: BackgroundService
        where TAggregateId : IIdentity
        where TFact : IFact

    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}