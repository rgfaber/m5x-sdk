using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.EventStore.Interfaces;
using Serilog;

namespace M5x.DEC.Infra.EventStore
{

    public class EsPlayerSettings
    {
        public Int64 Start { get; set; }
    }

    public interface IEventStorePlayer<TAggregateId>
        where TAggregateId : IIdentity
    {
        Task ReplayAsync();
    }
    
    public abstract class EventStorePlayer<TAggregateId> : IEventStorePlayer<TAggregateId>
        where TAggregateId: IIdentity, new()
    {
        private readonly IEsClient _client;
        private readonly IDECBus _bus;
        private readonly EsPlayerSettings _settings;
        private readonly ILogger _logger;
        private StreamSubscription _subscription;

        public EventStorePlayer(IEsClient client,
            IDECBus bus,
            EsPlayerSettings settings,
            ILogger logger)
        {
            _client = client;
            _bus = bus;
            _settings = settings;
            _logger = logger;
        }

        public async Task ReplayAsync()
        {
            try
            {
                var id = new TAggregateId();
                IEventFilter filter = StreamFilter.Prefix(id.GetIdPrefix());
                SubscriptionFilterOptions? filterOptions = new SubscriptionFilterOptions(
                    filter, 
                    1U, 
                    CheckpointReached);
                bool resolveLinkTos = false;
                _subscription = await _client.SubscribeToAllAsync(
                    Position.Start ,
                    EventAppeared,
                    resolveLinkTos,
                    SubscriptionDropped,
                    filterOptions,
                    ConfigureOperationOptions);
            }
            catch (Exception e)
            {
                _logger?.Error(e.InnerAndOuter());
            }
        }

        private void ConfigureOperationOptions(EventStoreClientOperationOptions operationOptions)
        { }

        private Task CheckpointReached(StreamSubscription subscription, Position position, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void SubscriptionDropped(StreamSubscription subscription
            , SubscriptionDroppedReason reason
            , Exception? exception)
        {
            _logger?.Debug($"Subscription [{subscription.SubscriptionId}] dropped. Reason: [{reason}] ");
        }

        private Task EventAppeared(StreamSubscription subscription, 
            ResolvedEvent resolvedEvent, 
            CancellationToken cancellationToken)
        {
            return _bus.PublishAsync(resolvedEvent);
        }
    }

}