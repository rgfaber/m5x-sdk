using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.EventStore.Interfaces;

namespace M5x.EventStore
{
    internal class EsPersistentSubscriptionsClient : IEsPersistentSubscriptionsClient
    {
        private readonly EventStorePersistentSubscriptionsClient _clt;

        public EsPersistentSubscriptionsClient(EventStorePersistentSubscriptionsClient clt)
        {
            _clt = clt;
        }

        public void Dispose()
        {
            _clt?
                .Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _clt
                .DisposeAsync();
        }

        public string ConnectionName => _clt.ConnectionName;

        public Task CreateAsync(string streamName, string groupName, PersistentSubscriptionSettings settings,
            UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
        {
            return _clt.CreateAsync(streamName, groupName, settings, userCredentials, cancellationToken);
        }

        public Task DeleteAsync(string streamName, string groupName, UserCredentials? userCredentials = null,
            CancellationToken cancellationToken = default)
        {
            return _clt.DeleteAsync(streamName, groupName, userCredentials, cancellationToken);
        }

        public Task<PersistentSubscription> SubscribeAsync(string streamName,
            string groupName,
            Func<PersistentSubscription, ResolvedEvent, int?, CancellationToken, Task> eventAppeared,
            Action<PersistentSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
            UserCredentials? userCredentials = null, int bufferSize = 10, bool autoAck = true,
            CancellationToken cancellationToken = default)
        {
            return _clt.SubscribeAsync(streamName,
                groupName,
                eventAppeared,
                subscriptionDropped,
                userCredentials,
                bufferSize,
                autoAck,
                cancellationToken);
        }

        public Task UpdateAsync(string streamName, string groupName, PersistentSubscriptionSettings settings,
            UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
        {
            return _clt.UpdateAsync(streamName, groupName, settings, userCredentials, cancellationToken);
        }
    }
}