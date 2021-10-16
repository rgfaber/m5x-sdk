using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Exceptions;
using EventStore.Client;
using EventStore.ClientAPI;
using M5x.CEQS.Persistence;
using M5x.EventStore;
using EventData = EventStore.Client.EventData;
using Position = EventStore.Client.Position;
using ResolvedEvent = EventStore.Client.ResolvedEvent;
using StreamPosition = EventStore.Client.StreamPosition;


namespace M5x.CEQS.Infra.EventStore
{
    public class EventPersistence : IEventPersistence
    {
        private readonly IEsClient _client;
        private readonly ILogger _logger;
        
        private class EventStoreEvent : ICommittedDomainEvent
        {
            public string AggregateId { get; set; }
            public string Data { get; set; }
            public string Metadata { get; set; }
            public int AggregateSequenceNumber { get; set; }
        }
        
        private static Position ParsePosition(GlobalPosition globalPosition)
        {
            if (globalPosition.IsStart)
            {
                return Position.Start;
            }

            var parts = globalPosition.Value.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException(string.Format(
                    "Unknown structure for global position '{0}'. Expected it to be empty or in the form 'L-L'",
                    globalPosition.Value));
            }

            var commitPosition = ulong.Parse(parts[0]);
            var preparePosition = ulong.Parse(parts[1]);

            return new Position(commitPosition, preparePosition);
        }
        

        public EventPersistence(IEsClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }
        
        
        public async Task<AllCommittedEventsPage> LoadAllCommittedEvents(GlobalPosition globalPosition, int pageSize, CancellationToken cancellationToken)
        {
            var nextPosition = ParsePosition(globalPosition);
            var resolvedEvents = await GetAllResolvedEvents(globalPosition, pageSize, cancellationToken);
            var eventStoreEvents = Map(resolvedEvents);
            return new AllCommittedEventsPage(
                new GlobalPosition(string.Format("{0}-{1}", nextPosition.CommitPosition, nextPosition.PreparePosition)),
                eventStoreEvents);
        }



        public async Task<IReadOnlyCollection<ICommittedDomainEvent>> CommitEventsAsync(IIdentity id,
            IReadOnlyCollection<SerializedEvent> serializedEvents,
            CancellationToken cancellationToken)
        {
            var committedDomainEvents = serializedEvents
                .Select(e => new EventStoreEvent
                {
                    AggregateSequenceNumber = e.AggregateSequenceNumber,
                    Metadata = e.SerializedMetadata,
                    AggregateId = id.Value,
                    Data = e.SerializedData
                })
                .ToList();
            var expectedVersion = Math.Max(serializedEvents.Min(e => e.AggregateSequenceNumber) - 2, ExpectedVersion.NoStream);
            var eventDatas = serializedEvents
                .Select(e =>
                {
                    // While it might be tempting to use e.Metadata.EventId here, we can't
                    // as EventStore won't detect optimistic concurrency exceptions then
                    var guid = Uuid.Parse(Guid.NewGuid().ToString());

                    var eventType = string.Format("{0}.{1}.{2}", e.Metadata[MetadataKeys.AggregateName], e.Metadata.EventName, e.Metadata.EventVersion);
                    var data = Encoding.UTF8.GetBytes(e.SerializedData);
                    var meta = Encoding.UTF8.GetBytes(e.SerializedMetadata);
                    return new EventData(guid, eventType, data, meta);
                });
            
            try
            {
                var expectedRevision = StreamState.Any;
                var writeResult = await _client.AppendToStreamAsync(id.Value,
                    expectedRevision,
                    eventDatas,
                    cancellationToken: cancellationToken);

                _logger.Debug(
                    "Wrote entity {0} with version {1} ({2},{3})",
                    id,
                    writeResult.NextExpectedStreamRevision-1,
                    writeResult.LogPosition.CommitPosition,
                    writeResult.LogPosition.PreparePosition);
            }
            catch (WrongExpectedVersionException e)
            {
                throw new OptimisticConcurrencyException(e.Message, e);
            }

            return committedDomainEvents;            
        }

        public async Task<IReadOnlyCollection<ICommittedDomainEvent>> LoadCommittedEventsAsync(IIdentity id,
            int fromEventSequenceNumber,
            CancellationToken cancellationToken)
        {
            var streamEvents =await GetResolvedEvents(id, fromEventSequenceNumber);
            return Map(streamEvents);
        }

        private async Task<IEnumerable<ResolvedEvent>> GetResolvedEvents(IIdentity id, int fromEventSequenceNumber)
        {
            var ret = new List<ResolvedEvent>();
            EventStoreClient.ReadStreamResult currentSlice;
            bool hasNext;
            var sliceSize = 200;
            StreamRevision nextRevision = StreamRevision.FromStreamPosition(StreamPosition.FromInt64(fromEventSequenceNumber));
            StreamPosition nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice =
                    _client.ReadStreamAsync(Direction.Forwards, id.Value, nextSliceStart, sliceSize);
                if (await currentSlice.ReadState == ReadState.StreamNotFound)
                    throw new EventStoreAggregateNotFoundException($"Aggregate {id.Value} not found");
                await foreach (var resolvedEvent in currentSlice)
                {
                    ret.Add(resolvedEvent);
                    nextSliceStart = resolvedEvent.Event.EventNumber.Next();
                }
                hasNext = (await currentSlice.MoveNextAsync());
            } while (hasNext);
            return ret;            
        }
        
        
        private async Task<IEnumerable<ResolvedEvent>> GetAllResolvedEvents(GlobalPosition globalPosition, int pageSize, CancellationToken cancellationToken)
        {
            IAsyncEnumerable<ResolvedEvent> currentSlice;
            bool hasNext;
            var sliceSize = 200;
            StreamPosition nextSliceStart = StreamPosition.Start;
            currentSlice = _client.ReadAllAsync(Direction.Forwards, ParsePosition(globalPosition), sliceSize, cancellationToken: cancellationToken);
            return await currentSlice.ToListAsync();            
        }

        
        
        
        
        
        
        
        
        
        
        
        

        public async Task DeleteEventsAsync(IIdentity id, CancellationToken cancellationToken)
        {
            await _client.SoftDeleteAsync(id.Value, StreamState.Any, cancellationToken: cancellationToken);
        }
        
        
        private static IReadOnlyCollection<EventStoreEvent> Map(IEnumerable<ResolvedEvent> resolvedEvents)
        {
            return resolvedEvents
                .Select(e => new EventStoreEvent
                {
                    AggregateSequenceNumber = Convert.ToInt32(e.Event.EventNumber.ToInt64() + 1), // Starts from zero
                    Metadata = Encoding.UTF8.GetString(e.Event.Metadata.Span),
                    AggregateId = e.OriginalStreamId,
                    Data = Encoding.UTF8.GetString(e.Event.Data.Span),
                })
                .ToList();
        }
        
        
        
    }
}