using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace M5x.EventStore.Interfaces;

public interface IEsClient : IEsClientBase
{
    Task<IWriteResult> AppendToStreamAsync(
        string streamName,
        StreamRevision expectedRevision,
        IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline=null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<IWriteResult> AppendToStreamAsync(
        string streamName,
        StreamState expectedState,
        IEnumerable<EventData> eventData,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline=null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    // Task<DeleteResult> SoftDeleteAsync(
    //     string streamName,
    //     StreamRevision expectedRevision,
    //     Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
    //     UserCredentials? userCredentials = null,
    //     CancellationToken cancellationToken = default);
    //
    // Task<DeleteResult> SoftDeleteAsync(
    //     string streamName,
    //     StreamState expectedState,
    //     Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
    //     UserCredentials? userCredentials = null,
    //     CancellationToken cancellationToken = default);

    Task<StreamMetadataResult> GetStreamMetadataAsync(
        string streamName,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<IWriteResult> SetStreamMetadataAsync(
        string streamName,
        StreamState expectedState,
        StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<IWriteResult> SetStreamMetadataAsync(
        string streamName,
        StreamRevision expectedRevision,
        StreamMetadata metadata,
        Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<ResolvedEvent> ReadAllAsync(
        Direction direction,
        Position position,
        long maxCount = 9223372036854775807,
        bool resolveLinkTos = false,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    EventStoreClient.ReadStreamResult ReadStreamAsync(
        Direction direction,
        string streamName,
        StreamPosition revision,
        long maxCount = 9223372036854775807,
        bool resolveLinkTos = false,
        TimeSpan? deadline=null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<StreamSubscription> SubscribeToAllAsync(
        FromAll startFrom,
        Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
        bool resolveLinkTos = false,
        Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
        SubscriptionFilterOptions? filterOptions = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    // Task<StreamSubscription> SubscribeToAllAsync(
    //     Position start,
    //     Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
    //     bool resolveLinkTos = false,
    //     Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
    //     SubscriptionFilterOptions? filterOptions = null,
    //     Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
    //     UserCredentials? userCredentials = null,
    //     CancellationToken cancellationToken = default);

    // Task<StreamSubscription> SubscribeToStreamAsync(
    //     string streamName,
    //     Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
    //     bool resolveLinkTos = false,
    //     Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
    //     Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
    //     UserCredentials? userCredentials = null,
    //     CancellationToken cancellationToken = default);

    Task<StreamSubscription> SubscribeToStreamAsync(
        string streamName,
        FromStream start,
        Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
        bool resolveLinkTos = false,
        Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<DeleteResult> TombstoneAsync(
        string streamName,
        StreamRevision expectedRevision,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);

    Task<DeleteResult> TombstoneAsync(
        string streamName,
        StreamState expectedState,
        TimeSpan? deadline = null,
        UserCredentials? userCredentials = null,
        CancellationToken cancellationToken = default);
}