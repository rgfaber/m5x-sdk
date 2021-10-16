using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Nats;
using M5x.Schemas;
using Microsoft.Extensions.Hosting;
using MyNatsClient;
using MyNatsClient.Rx;
using Serilog;

namespace M5x.DEC.Infra.NATS
{
    public abstract class NATSListener<TAggregateId, TEvent> : BackgroundService
        where TAggregateId : IAggregateID
        where TEvent : IEvent<TAggregateId>
    {
        private readonly INatsClient _bus;
        private readonly IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> _handlers;
        private readonly ILogger _logger;

        private string _logMessage;
        private ISubscription _subscription;

        protected NATSListener(INatsClient bus,
            IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> handlers,
            ILogger logger)
        {
            _bus = bus;
            _handlers = handlers;
            _logger = logger;
        }

        private string EventTopic => GetTopic();

        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TEvent).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TEvent)}!");
            return atts[0].Id;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartProcessing(cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopProcessing(cancellationToken);
        }

        private async Task StopProcessing(CancellationToken cancellationToken)
        {
            if (_bus.IsConnected)
            {
                _logMessage = $"::UNSUBSCRIBING ::Topic: [{EventTopic}]";
                _logger?.Debug(_logMessage);
                await _bus.UnsubAsync(_subscription);
                _logMessage = $"::DISCONNECTING ::Bus: [{_bus.Id}]";
                _logger?.Debug(_logMessage);
                _bus.Disconnect();
            }

            ;
        }


        private async Task StartProcessing(CancellationToken cancellationToken)
        {
            try
            {
                var request = string.Empty;
                if (!_bus.IsConnected)
                {
                    _logger?.Debug($"::CONNECT ::Bus [{_bus?.Id}]");
                    await _bus.ConnectAsync();
                }

                _logger?.Debug($"::SUBSCRIBE ::Topic: [{EventTopic}] on bus [{_bus?.Id}]");
                _subscription = await _bus.SubAsync(EventTopic,
                    stream => stream.SubscribeSafe(
                        async msg =>
                        {
                            var evt = msg.Payload.NatsDecode<TEvent>();
                            request = $"{EventTopic} - [{evt.EventId}]";
                            _logger?.Debug($"::RECEIVED: {request} ::Payload: {evt.EventId}");
                            foreach (var handler in _handlers)
                            {
                                _logger?.Debug($"::HANDLING ::Event: {evt.EventId} Handler: {handler.GetType()}");
                                await handler.HandleAsync(evt);
                            }
                        },
                        exception => { _logger?.Error($"::ERROR: {request}  {exception.Message} "); },
                        () => { _logger?.Debug($"::COMPLETED: {request}"); }));
            }
            catch (Exception e)
            {
                _logger?.Fatal($"::EXCEPTION {e.Message})");
                throw;
            }
        }
    }
}