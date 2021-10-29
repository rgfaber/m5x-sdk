using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RMqAsyncResponder<TID, THope, TCmd, TFeedback> 
        : BackgroundService, IResponder<TID, THope, TCmd, TFeedback>
        where TID : IIdentity
        where TCmd : ICommand<TID>
        where THope : IHope
        where TFeedback : IFeedback
    {
        private readonly IAsyncActor<TID, TCmd, TFeedback> _actor;
        private IModel _channel;
        private readonly IConnection _connection;
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private AsyncEventingBasicConsumer _hopeConsumer;

        protected RMqAsyncResponder(
            IConnection connection,
            IAsyncActor<TID, TCmd, TFeedback> actor,
            ILogger logger)
        {
            _actor = actor;
            _logger = logger;
            _connection = connection;
        }


        public string Topic => GetTopic();

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(Topic, false, false, false, null);
            _channel.BasicQos(0, 1, false);
            _hopeConsumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(Topic, false, _hopeConsumer);
            _hopeConsumer.Received += HopeReceived ;
            return Task.CompletedTask;
        }

        private async Task HopeReceived(object sender, BasicDeliverEventArgs ea)
        {
            var feedback = (TFeedback)Activator.CreateInstance(typeof(TFeedback));
            var replyProps = _channel.CreateBasicProperties();
            var props = ea.BasicProperties;
            replyProps.CorrelationId = props.CorrelationId;
            try
            {
                var hope = JsonSerializer.Deserialize<THope>(ea.Body.Span);
                feedback = await _actor.HandleAsync(ToCommand(hope));
            }
            catch (Exception e)
            {
                _logger?.Error(e.InnerAndOuter());
                feedback.ErrorState.Errors.Add("RabbitMQ.ResponderError", e);
            }
            finally
            {
                var responseBytes = JsonSerializer.SerializeToUtf8Bytes(feedback);
                _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        }

        protected abstract TCmd ToCommand(THope hope);


        private string GetTopic()
        {
            var attrs = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : throw new Exception($"No Topic Defined on {typeof(THope)}!");
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
            return Task.CompletedTask;
        }
    }
}