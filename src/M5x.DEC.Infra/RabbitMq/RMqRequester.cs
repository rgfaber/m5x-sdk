using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Humanizer;
using M5x.DEC.Schema;
using MassTransit;
using Polly;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RMqRequester<THope, TFeedback>: IRequester<THope, TFeedback> 
        where THope : class, IHope 
        where TFeedback : class, IFeedback
    {
        protected IRequestClient<THope> Client { get; }

        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly AsyncRetryPolicy _retryPolicy;
        private string _logMessage;
        
        public string HopeTopic => GetHopeTopic();
        
        protected virtual string GetHopeTopic()
        {
            var attrs = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(THope)}!";
        }


        public async Task<TFeedback> RequestAsync(THope hope, CancellationToken cancellationToken=default)
        {
            var fbk = (TFeedback)Activator.CreateInstance(typeof(TFeedback));
            Guard.Against.Null(fbk, nameof(fbk));
            var res = await Client
                .GetResponse<TFeedback>(hope, cancellationToken)
                .ConfigureAwait(false);
            Guard.Against.Null(res, nameof(res));
            fbk = res.Message;
            return fbk;
        }
        
        
        public RMqRequester(IRequestClient<THope> client,ILogger logger, AsyncRetryPolicy retryPolicy = null)
        {
            _logger = logger;
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * 100));
            Client = client;
        }
        
        


    }
}