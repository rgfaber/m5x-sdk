using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.DEC.Events;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.Schema;
using M5x.EventStore.Interfaces;
using Polly;
using Polly.Retry;

namespace M5x.DEC.Infra.EventStore;

internal class EventStoreDb : IEventStore
{
    private readonly IEsClient _client;
    private readonly int _maxRetries = Polly.Config.MaxRetries;
    private readonly AsyncRetryPolicy _retryPolicy;


    public EventStoreDb(IEsClient client, AsyncRetryPolicy retryPolicy = null)
    {
        _client = client;
        _retryPolicy = retryPolicy
                       ?? Policy
                           .Handle<Exception>()
                           .WaitAndRetryAsync(_maxRetries,
                               times => TimeSpan.FromMilliseconds(times * 100));
    }

    public async Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsAsync<TAggregateId>(TAggregateId id)
        where TAggregateId : IIdentity
    {
        var ret = new List<StoreEvent<TAggregateId>>();
        var events = _client.ReadStreamAsync(Direction.Forwards, id.Value, StreamPosition.Start);
        var state = await events.ReadState.ConfigureAwait(false);
        if (state == ReadState.StreamNotFound) return null;
        await foreach (var @event in events)
        {
            var s = SerializationHelper.Deserialize<TAggregateId>(@event.Event.EventType,
                @event.Event.Data.ToArray());

            var ev = new StoreEvent<TAggregateId>(s, @event.Event.EventNumber.ToInt64());

            ret.Add(ev);
        }

        return ret;
    }

    public async IAsyncEnumerable<StoreEvent<TAggregateId>> ReadAllEventsAsync<TAggregateId>(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TAggregateId : IIdentity
    {
        var ret = new List<StoreEvent<TAggregateId>>();
        var events = _client.ReadAllAsync(Direction.Forwards, Position.Start, cancellationToken: cancellationToken);
        var enumerator = events.GetAsyncEnumerator(cancellationToken);
        do
        {
            var id = (IIdentity)Activator.CreateInstance(typeof(TAggregateId));
            var prefix = id.GetIdPrefix();
            var resolvedEvent = enumerator.Current;
            if (!resolvedEvent.Event.EventStreamId.StartsWith(prefix)) continue;
            var devt = SerializationHelper.Deserialize<TAggregateId>(resolvedEvent.Event.EventType,
                resolvedEvent.Event.Data.ToArray());

            var ev = devt.ToStoreEvent(resolvedEvent.Event.EventNumber.ToInt64());
//                var ev = new StoreEvent<TAggregateId>(devt, resolvedEvent.Event.EventNumber.ToInt64());

            yield return ev;
        } while (await enumerator.MoveNextAsync());
    }

    public async Task<AppendResult> AppendEventAsync<TAggregateId>(IEvent<TAggregateId> @event)
        where TAggregateId : IIdentity
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var typeName = @event.GetType().AssemblyQualifiedName;

            var eventData = new EventData(
                Uuid.FromGuid(@event.EventId),
                typeName, SerializationHelper.Serialize(@event),
                JsonSerializer.SerializeToUtf8Bytes(@event.Meta));
//                    Encoding.UTF8.GetBytes(@event.Meta.ToString()));
            var expectedRevision = StreamRevision.None;
            if (@event.Meta.Version == AggregateRoot<TAggregateId>.NewAggregateVersion)
                expectedRevision = StreamRevision.FromInt64(@event.Meta.Version);

            var writeResult = await _client.AppendToStreamAsync(@event.Meta.Id,
                StreamState.Any,
                new[] { eventData });

            // TODO : Check if this works with streamrevision
            // writeResult = await _client.AppendToStreamAsync(@event.Meta.Id,
            //     StreamRevision.FromInt64(@event.Meta.Version), 
            //     new[] {eventData});

            return AppendResult.New(writeResult.NextExpectedStreamRevision.ToInt64());
        });
    }


    public async Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsSlicedAsync<TAggregateId>(TAggregateId id,
        int sliceSize, long startPos)
        where TAggregateId : IIdentity
    {
        var ret = new List<StoreEvent<TAggregateId>>();
        EventStoreClient.ReadStreamResult currentSlice;
        bool hasNext;
        var nextSliceStart = StreamPosition.FromInt64(startPos);
        do
        {
            currentSlice =
                _client.ReadStreamAsync(Direction.Forwards, id.Value, nextSliceStart, sliceSize);
            if (await currentSlice.ReadState == ReadState.StreamNotFound)
                throw new EventStoreAggregateNotFoundException($"Aggregate {id.Value} not found");
            await foreach (var resolvedEvent in currentSlice)
            {
                var s = SerializationHelper.Deserialize<TAggregateId>(resolvedEvent);
                var ev = new StoreEvent<TAggregateId>(s, resolvedEvent.Event.EventNumber.ToInt64());
                ret.Add(ev);
                nextSliceStart = resolvedEvent.Event.EventNumber.Next();
            }

            hasNext = await currentSlice.MoveNextAsync();
        } while (hasNext);

        return ret;
    }
}