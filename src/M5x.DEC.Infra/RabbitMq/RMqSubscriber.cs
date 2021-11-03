using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public static class Guards
    {
        public static void NoFactHandlersRegistered<TAggregateId, TFact>( this IGuardClause clause,
            IEnumerable<IFactHandler<TAggregateId, TFact>> handlers, string paramName=null) 
            where TFact : IFact
            where TAggregateId : IIdentity
        {
            if (handlers == null)
                throw new ArgumentException($"No Fact Handlers Registered for {typeof(TFact).PrettyPrint()}", paramName);
            if(!handlers.Any())
                throw new ArgumentException($"No Fact Handlers Registered for {typeof(TFact).PrettyPrint()}", paramName);
        }
    }
    
    public abstract class RMqSubscriber<TAggregateId, TFact> : BackgroundService, 
        ISubscriber<TAggregateId, TFact>
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        private readonly IConnectionFactory _connFact;
        private readonly IDECBus _bus;
        private IModel _channel;
        private IConnection _connection;
        private readonly IEnumerable<IFactHandler<TAggregateId, TFact>> _handlers;
        private readonly ILogger _logger;


        protected RMqSubscriber(
            IConnectionFactory connFact,
            IDECBus bus,
            IEnumerable<IFactHandler<TAggregateId, TFact>> handlers,
            ILogger logger)
        {
            _connFact = connFact;
            _bus = bus;
            _handlers = handlers;
            _logger = logger;
        }

        public string Topic => GetTopic();

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = _connFact.CreateConnection();
            _channel = _connection.CreateModel();
            _logger?.Debug($"[{Topic}]-SUB [{GetType().PrettyPrint()}]");
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
            try
            {
                Guard.Against.Null(ea, nameof(ea));
                Guard.Against.Null(ea.Body, nameof(ea.Body));
                _bus.Subscribe<TFact>(HandleFactAsync);
                var fact = JsonSerializer.Deserialize<TFact>(ea.Body.Span);
                _logger?.Debug($"[{Topic}]-RCV Fact({fact.CorrelationId})");
                return _bus.PublishAsync(fact);
            }
            catch (Exception e)
            {
                _logger?.Error(e.InnerAndOuter());
                return Task.CompletedTask;
            }
        }

        private Task HandleFactAsync(TFact fact)
        {
            try
            {
                Guard.Against.NoFactHandlersRegistered(_handlers, nameof(_handlers));
                Guard.Against.Null(fact, nameof(fact));
                foreach (var handler in _handlers) 
                    handler.HandleAsync(fact);
            }
            catch (Exception e)
            {
                _logger?.Error(e.InnerAndOuter());
            }
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
            _connection?.Dispose();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}