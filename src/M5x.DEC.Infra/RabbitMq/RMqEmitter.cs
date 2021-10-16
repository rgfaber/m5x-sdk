using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using MassTransit;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public class RMqEmitter<TAggregateId, TFact> : IFactEmitter<TAggregateId,TFact> 
        where TAggregateId : IIdentity 
        where TFact : IFact
    {
        private readonly IPublishEndpoint _endpoint;


        public RMqEmitter(IPublishEndpoint endpoint,
        ILogger logger, AsyncRetryPolicy retryPolicy = null)
        {
            _endpoint = endpoint;
        }



        public async Task EmitAsync(TFact fact, CancellationToken cancellationToken)
        {
            await _endpoint.Publish(fact, cancellationToken);
        }
    }
}