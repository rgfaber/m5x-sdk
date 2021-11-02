using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RMqEmitter<TAggregateId, TEvent, TFact> 
      : IEventHandler<TAggregateId,TEvent>, IDisposable
//    : IFactEmitter<TAggregateId, TEvent, TFact>, IDisposable
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
        where TFact : IFact
    {
        private readonly int _backoff = 100;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly AsyncRetryPolicy _retryPolicy;


        protected RMqEmitter(
            IConnection connection,
            ILogger logger,
            AsyncRetryPolicy retryPolicy = null)
        {
            _logger = logger;
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * _backoff));
            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(Topic, ExchangeType.Fanout);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }


        public string Topic => GetTopic();


        public Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            var fact = ToFact(@event);
            _logger?.Debug($"[{Topic}]-EMIT Fact[{fact.CorrelationId}]");
            var body = JsonSerializer.SerializeToUtf8Bytes(fact);
            _channel.BasicPublish(Topic, "", null, body);
            return Task.CompletedTask;
        }

        private string GetTopic()
        {
            var attrs = (TopicAttribute[])typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : throw new Exception($"No Topic Defined on {typeof(TFact)}!");
        }

        protected abstract TFact ToFact(TEvent @event);
    }
}