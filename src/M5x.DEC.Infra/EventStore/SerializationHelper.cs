using System;
using System.Text;
using EventStore.Client;
using M5x.DEC.Events;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.Schema;
using M5x.EventStore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.DEC.Infra.EventStore
{
    public static class SerializationHelper
    {
        public static IEvent<TAggregateId> Deserialize<TAggregateId>(string eventType, byte[] data)
            where TAggregateId : IIdentity
        {
            try
            {
                var settings = new JsonSerializerSettings
                    { ContractResolver = new PrivateSetterContractResolver() };
                return (IEvent<TAggregateId>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data),
                    Type.GetType(eventType), settings);

                // // var settings = new JsonSerializerSettings {ContractResolver = new PrivateSetterContractResolver()};
                // // return (IEvent<TAggregateId>) JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data),
                // //     Type.GetType(eventType), settings);
                //
                // // var settings = new JsonSerializerSettings {ContractResolver = new PrivateSetterContractResolver()};
           //     return (IEvent<TAggregateId>)JsonSerializer.Deserialize(data, Type.GetType(eventType));
                // //  JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data),
                // // Type.GetType(eventType), settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new EventStoreDeserializationException($"Failed to Deserialize eventType {eventType}", data,
                    e);
            }
        }

        public static byte[] Serialize<TAggregateId>(IEvent<TAggregateId> @event) where TAggregateId : IIdentity
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            //return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            //return JsonSerializer.SerializeToUtf8Bytes(@event);
        }


        public static StoreEvent<TAggregateId> ToStoreEvent<TAggregateId>(this IEvent<TAggregateId> @event,
            long eventNumber)
            where TAggregateId : IIdentity
        {
            return new StoreEvent<TAggregateId>(@event, eventNumber);
        }

        public static IEvent<TAggregateId> Deserialize<TAggregateId>(ResolvedEvent resolvedEvent)
            where TAggregateId : IIdentity
        {
            return Deserialize<TAggregateId>(resolvedEvent.Event.EventType, resolvedEvent.Event.Data.ToArray());
        }
    }
}