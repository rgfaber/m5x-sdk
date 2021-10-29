using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RmqSubscriber<TAggregateId, TFact> : BackgroundService, 
        ISubscriber<TAggregateId, TFact>
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        private readonly IDECBus _bus;
        private IModel _channel;
        private readonly IConnection _connection;
        private readonly IEnumerable<IFactHandler<TAggregateId, TFact>> _handlers;
        private readonly ILogger _logger;


        protected RmqSubscriber(
            IConnection connection,
            IDECBus bus,
            IEnumerable<IFactHandler<TAggregateId, TFact>> handlers,
            ILogger logger)
        {
            _connection = connection;
            _bus = bus;
            _handlers = handlers;
            _logger = logger;
            _channel = _connection.CreateModel();
        }

        public string Topic => GetTopic();

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel.ExchangeDeclare(Topic, ExchangeType.Fanout);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, Topic, "");
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += FactReceived;
            _channel.BasicConsume(queueName, true, consumer);
            return Task.CompletedTask;
        }

        private string GetTopic()
        {
            var attrs = (TopicAttribute[])typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : throw new Exception($"No Topic Defined on {typeof(TFact)}!");
        }

        private Task FactReceived(object sender, BasicDeliverEventArgs ea)
        {
            _bus.Subscribe<TFact>(HandleFactAsync);
            var fact = JsonSerializer.Deserialize<TFact>(ea.Body.Span);
            return _bus.PublishAsync(fact);
        }

        private Task HandleFactAsync(TFact fact)
        {
            foreach (var handler in _handlers) 
                handler.HandleAsync(fact);
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            { }
            return Task.CompletedTask;
        }


        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}