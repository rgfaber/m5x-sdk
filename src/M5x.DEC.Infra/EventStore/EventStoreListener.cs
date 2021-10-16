using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.DEC.Events;
using M5x.DEC.Schema;
using M5x.EventStore;
using M5x.EventStore.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace M5x.DEC.Infra.EventStore
{
    public class EventStoreListener<TAggregateId, TEvent> : BackgroundService
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
    {
        private readonly IEsClient _connection;
        private readonly IEnumerable<IEventHandler<TAggregateId, TEvent>> _handlers;
        private readonly ILogger _logger;
        private readonly bool _resolveLinkTos = true;
        private bool _isConnected;
        private string _logMessage;

        public EventStoreListener(IEsClient connection,
            IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers,
            ILogger logger)
        {
            _logger = logger;
            _connection = connection;
            _handlers = handlers;
        }

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
                var subscription = await _connection.SubscribeToAllAsync(EventAppeared, false, SubscriptionDropped,
                    cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger?.Fatal($"::EXCEPTION {e.Message})");
                throw;
            }
        }


        private void SubscriptionDropped(StreamSubscription sub, SubscriptionDroppedReason reason, Exception? exception)
        {
        }

        private Task EventAppeared(StreamSubscription sub, ResolvedEvent evt, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopListeningAsync(cancellationToken);
        }


        private async Task StopListeningAsync(CancellationToken cancellationToken)
        {
        }
    }
}