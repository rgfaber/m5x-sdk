using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.EventStore.Interfaces;

namespace M5x.EventStore;

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
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null, CancellationToken cancellationToken = default)
    {
        return _clt.CreateAsync(streamName, groupName, settings, deadline, userCredentials, cancellationToken);
    }

    public Task DeleteAsync(string streamName, string groupName, 
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default)
    {
        return _clt.DeleteAsync(streamName, groupName, deadline, userCredentials, cancellationToken);
    }

    public Task<PersistentSubscription> SubscribeAsync(
        string streamName,
        string groupName,
        Func<PersistentSubscription, ResolvedEvent, int?, CancellationToken, Task> eventAppeared,
        Action<PersistentSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
        UserCredentials? userCredentials = null, 
        int bufferSize = 10, 
        CancellationToken cancellationToken = default)
    {
        return _clt.SubscribeToStreamAsync(
            streamName,
            groupName,
            eventAppeared,
            subscriptionDropped,
            userCredentials,
            bufferSize,
            cancellationToken);
    }

    public Task UpdateAsync(string streamName, string groupName, 
        PersistentSubscriptionSettings settings,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null, 
        CancellationToken cancellationToken = default)
    {
        return _clt.UpdateAsync(streamName, groupName, settings, deadline, userCredentials, cancellationToken);
    }
}