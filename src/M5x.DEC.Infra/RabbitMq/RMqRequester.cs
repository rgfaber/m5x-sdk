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
        where TFeedback : class, IFeedback
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly IConnectionFactory _connFact;
        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private IBasicProperties _props;
        private BlockingCollection<TFeedback> _responseQ;
        private readonly AsyncRetryPolicy _retryPolicy;
        private AsyncEventingBasicConsumer _rspConsumer;
        private string _rspQName;
        private string _correlationId;
        private string _logMessage;

        protected RMqRequester(IConnectionFactory connFact,
            ILogger logger, AsyncRetryPolicy retryPolicy = null)
        {
            _connFact = connFact;
            _logger = logger;
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * 100));
            // Setup RabbitMQ RPC
            // reference: https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html
        }
        public string Topic => GetTopic();
        public Task<TFeedback> RequestAsync(THope hope, CancellationToken cancellationToken = default)
        {
            _connection = _connFact.CreateConnection();
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

            var fbk = Activator.CreateInstance<TFeedback>();
            Guard.Against.Null(fbk, nameof(fbk));
            Guard.Against.Null(hope, nameof(hope));
            Guard.Against.NullOrEmpty(hope.CorrelationId, nameof(hope.CorrelationId));
            _correlationId = hope.CorrelationId;
            _correlationId = GuidUtils.NewGuid;
            _props.CorrelationId = _correlationId;
            _props.ReplyTo = _rspQName;
            var body = JsonSerializer.SerializeToUtf8Bytes(hope);
            _logger?.Debug($"[{Topic}]-REQ Hope[{hope.CorrelationId}]");
            _channel.BasicPublish("",
                Topic,
                _props,
                body);
            fbk = _responseQ.Take(cancellationToken);
            var ok = fbk.IsSuccess ? "Success" : "Failure";
            _logger?.Debug($"[{Topic}]-FBK Feedback[{fbk.CorrelationId} -- ({ok})]");
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