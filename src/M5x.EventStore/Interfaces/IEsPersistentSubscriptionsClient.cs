using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace M5x.EventStore.Interfaces
{
  /// <summary>
  ///     The client used to manage persistent subscriptions in the EventStoreDB.
  /// </summary>
  /// <footer>
  ///     <a href="https://www.google.com/search?q=EventStore.Client.EventStorePersistentSubscriptionsClient">
  ///         `EventStorePersistentSubscriptionsClient`
  ///         on google.com
  ///     </a>
  /// </footer>
  public interface IEsPersistentSubscriptionsClient : IEsClientBase
    {
        public Task CreateAsync(
            string streamName,
            string groupName,
            PersistentSubscriptionSettings settings,
            UserCredentials? userCredentials = null,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string streamName,
            string groupName,
            UserCredentials? userCredentials = null,
            CancellationToken cancellationToken = default);

        Task<PersistentSubscription> SubscribeAsync(
            string streamName,
            string groupName,
            Func<PersistentSubscription, ResolvedEvent, int?, CancellationToken, Task> eventAppeared,
            Action<PersistentSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
            UserCredentials? userCredentials = null,
            int bufferSize = 10,
            bool autoAck = true,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            string streamName,
            string groupName,
            PersistentSubscriptionSettings settings,
            UserCredentials? userCredentials = null,
            CancellationToken cancellationToken = default);
    }
}