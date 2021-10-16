using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using M5x.Schemas;
using Microsoft.Extensions.Hosting;
using ILogger = Serilog.ILogger;

namespace M5x.DEC.Infra.EventStore
{
    public class EventStoreListener<TAggregateId, TEvent> : BackgroundService
        where TAggregateId : IAggregateID
        where TEvent : IEvent<TAggregateId>
    {
        private readonly IEventStoreConnection _connection;
        private readonly IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> _handlers;
        private readonly ILogger _logger;
        private readonly bool _resolveLinkTos = true;
        private bool _isConnected;
        private string _logMessage;

        public EventStoreListener(IEventStoreConnection connection,
            IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> handlers,
            ILogger logger)
        {
            _logger = logger;
            _connection = connection;
            _handlers = handlers;
            _connection.Connected += (sender, args) => { _isConnected = true; };
            _connection.Disconnected += (sender, args) => { _isConnected = false; };
            _connection.Closed += (sender, args) =>
            {
                _isConnected = false;
                _logMessage = $"::CONNECTION CLOSED ::Stream: [{EventStream}]";
                _logger?.Debug(_logMessage);
            };
        }


        public string EventStream => GetEventStream();


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }


        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartListening(cancellationToken);
        }

        private async Task StartListening(CancellationToken cancellationToken)
        {
            try
            {
                var request = string.Empty;
                if (!_isConnected)
                {
                    _logger?.Debug($"::CONNECT ::EventStore [{_connection?.ConnectionName}]");
                    await _connection?.ConnectAsync();
                }

                var subscription = await _connection.SubscribeToAllAsync(_resolveLinkTos
                    , (storeSubscription, @event) =>
                    {
                        // TODO: Explore how this works.
                    }
                    , (storeSubscription, reason, ex) => { }
                );


                //      _logger?.Debug($"::SUBSCRIBE ::Stream: [{EventTopic}] on EventStore [{_connection?.ConnectionName}]");


                // _subscription = await _bus.SubAsync(EventTopic,
                //     stream => stream.SubscribeSafe(
                //         async msg =>
                //         {
                //             var evt = msg.Payload.NatsDecode<TEvent>();
                //             request = $"{EventTopic} - [{evt.EventId}]";
                //             _logger?.Debug($"::RECEIVED: {request} ::Payload: {evt.EventId}");
                //             foreach (var handler in _handlers)
                //             {
                //                 _logger?.Debug($"::HANDLING ::Event: {evt.EventId} Handler: {handler.GetType()}");
                //                 await handler.HandleAsync(evt);
                //             }
                //         },
                //         exception => { _logger?.Error($"::ERROR: {request}  {exception.Message} "); },
                //         () => { _logger?.Debug($"::COMPLETED: {request}"); }));
            }
            catch (Exception e)
            {
                _logger?.Fatal($"::EXCEPTION {e.Message})");
                throw;
            }
        }

        private void SubscriptionDropped(EventStoreSubscription arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            throw new NotImplementedException();
        }

        private Task EventAppeared(EventStoreSubscription arg1, ResolvedEvent arg2)
        {
            throw new NotImplementedException();
        }

        private string GetEventStream()
        {
            throw new NotImplementedException();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopListeningAsync(cancellationToken);
        }


        private async Task StopListeningAsync(CancellationToken cancellationToken)
        {
            if (_isConnected)
            {
                _logMessage = $"::UNSUBSCRIBING ::EventStream: [{EventStream}]";
                _logger?.Debug(_logMessage);
                _connection.Close();
            }

            ;
        }
    }
}