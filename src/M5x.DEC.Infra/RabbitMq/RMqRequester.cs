using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RMqRequester<THope, TFeedback> :
        IRequester<THope, TFeedback>
        where THope : class, IHope
        where TFeedback : class, IExecutionResult
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<TFeedback> _responseQ;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncEventingBasicConsumer _rspConsumer;
        private readonly string _rspQName;
        private string _correlationId;
        private string _logMessage;

        protected RMqRequester(IConnection connection,
            ILogger logger, AsyncRetryPolicy retryPolicy = null)
        {
            _connection = connection;
            _logger = logger;
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * 100));
            // Setup RabbitMQ RPC
            // reference: https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html

            _channel = _connection.CreateModel();
            _rspConsumer = new AsyncEventingBasicConsumer(_channel);
            _props = _channel.CreateBasicProperties();
            _rspQName = _channel.QueueDeclare().QueueName;
            _responseQ = new BlockingCollection<TFeedback>();
            _rspConsumer.Received += RspConsumerOnReceived;
            _channel.BasicConsume(
                _rspConsumer,
                _rspQName,
                true);
        }
        public string Topic => GetTopic();
        public Task<TFeedback> RequestAsync(THope hope, CancellationToken cancellationToken = default)
        {
            var fbk = Activator.CreateInstance<TFeedback>();
            Guard.Against.Null(fbk, nameof(fbk));
            Guard.Against.Null(hope, nameof(hope));
            Guard.Against.NullOrEmpty(hope.CorrelationId, nameof(hope.CorrelationId));
            _correlationId = hope.CorrelationId;
            _correlationId = GuidUtils.NewGuid;
            _props.CorrelationId = _correlationId;
            _props.ReplyTo = _rspQName;
            var body = JsonSerializer.SerializeToUtf8Bytes(hope);
            _channel.BasicPublish("",
                Topic,
                _props,
                body);
            fbk = _responseQ.Take(cancellationToken);
            return Task.FromResult(fbk);
        }
        private Task RspConsumerOnReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var rsp = JsonSerializer.Deserialize<TFeedback>(body.Span);
            if (ea.BasicProperties.CorrelationId == _correlationId)
                _responseQ.Add(rsp);
            return Task.CompletedTask;
        }
        private string GetTopic()
        {
            var attrs = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0
                ? attrs[0].Id
                : throw new ArgumentException($"No Topic Defined on {typeof(THope)}!");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _responseQ?.Dispose();
        }
    }
}