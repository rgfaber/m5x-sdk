using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.DEC.Persistence.EventStore;
using M5x.EventStore;
using M5x.Schemas;
using Newtonsoft.Json;


namespace M5x.DEC.Infra.EventStore
{
    public abstract class EventStream<TAggregateID, TEvent> : IEventStore
        where TAggregateID: IAggregateID
        where TEvent: IEvent<TAggregateID>
    {
        private readonly IEsClient _client;

        public EventStream(IEsClient client)
        {
            _client = client;

        }

        public async Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsAsync<TAggregateId>(TAggregateId id)
            where TAggregateId : IAggregateID
        {
            var ret = new List<StoreEvent<TAggregateId>>();
            var events = _client.ReadStreamAsync(Direction.Forwards, id.Id, StreamPosition.Start);
            if (await events.ReadState == ReadState.StreamNotFound) return null;
//                throw new EventStoreException($"Event Stream {id.Id} was not found.");
            await foreach (var @event in events)
            {
                var s = Deserialize<TAggregateId>(@event.Event.Data.ToArray());
                var ev = new StoreEvent<TAggregateId>(s, @event.Event.EventNumber.ToInt64());
                ret.Add(ev);
            }
            return ret;
        }
        
        public async Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsSlicedAsync<TAggregateId>(TAggregateId id, int sliceSize)
        where TAggregateId : IAggregateID {
                var ret = new List<StoreEvent<TAggregateId>>();
                EventStoreClient.ReadStreamResult currentSlice;
                bool hasNext;
                StreamRevision nextRevision = StreamRevision.FromStreamPosition(StreamPosition.Start);
                StreamPosition nextSliceStart = StreamPosition.Start;
                do
                {
                    currentSlice =
                        _client.ReadStreamAsync(Direction.Forwards, id.Value, nextSliceStart, sliceSize);
                    if (await currentSlice.ReadState == ReadState.StreamNotFound)
                        throw new EventStoreAggregateNotFoundException($"Aggregate {id.Value} not found");
                    await foreach (var resolvedEvent in currentSlice)
                    {
//                        var s = Deserialize<TAggregateId>(resolvedEvent.Event.EventType, resolvedEvent.Event.Data.ToArray());
                        var s = Deserialize<TAggregateId>(resolvedEvent.Event.Data.ToArray());
                        var ev = new StoreEvent<TAggregateId>(s, resolvedEvent.Event.EventNumber.ToInt64());
                        ret.Add(ev);
                        nextSliceStart = resolvedEvent.Event.EventNumber.Next();
                    }
                    hasNext = (await currentSlice.MoveNextAsync());
                } while (hasNext);
                return ret;
        }

        public async Task<AppendResult> AppendEventAsync<TAggregateId>(IEvent<TAggregateId> @event)
            where TAggregateId : IAggregateID
        {
            try
            {
                var eventData = new EventData(
                    Uuid.FromGuid(@event.EventId),
                    @event.GetType().AssemblyQualifiedName,
                    Serialize(@event),
                    Encoding.UTF8.GetBytes("{}"));
                var expectedRevision = StreamRevision.None;
                if (@event.AggregateVersion == AggregateRoot<TAggregateId>.NewAggregateVersion) expectedRevision = StreamRevision.FromInt64(@event.AggregateVersion);

                var writeResult = await _client.AppendToStreamAsync(@event.AggregateId.Value,
                    StreamState.Any,
                    new[] {eventData});
                
                // var writeResult = await _client.AppendToStreamAsync( @event.AggregateId.Value,
                //     expectedRevision,
                //     new[] {eventData});
                
                return new AppendResult(writeResult.NextExpectedStreamRevision.ToInt64());
            }
            catch (Exception ex)
            {
                throw new EventStoreCommunicationException(
                    $"Error while appending event {@event.EventId} for aggregate {@event.AggregateId}", ex);
            }
        }

        private IEvent<TAggregateId> Deserialize<TAggregateId>(byte[] data)
        {
            try
            {
                var settings = new JsonSerializerSettings {ContractResolver = new PrivateSetterContractResolver()};
                return (IEvent<TAggregateId>) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data),
                    typeof(TEvent), settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new EventStoreDeserializationException($"Failed to Deserialize eventTyp {typeof(TEvent)}", data, e);
            }
        }

        private byte[] Serialize<TAggregateId>(IEvent<TAggregateId> @event)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
        }


    }
}