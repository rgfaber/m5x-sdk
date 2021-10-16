using System;
using System.Runtime.Serialization;

namespace M5x.DEC.Persistence.EventStore
{
    [Serializable]
    public class EventStoreException : Exception
    {
        public EventStoreException()
        {
        }

        public EventStoreException(string message) : base(message)
        {
        }

        public EventStoreException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventStoreException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


    [Serializable]
    public class EventStoreAggregateNotFoundException : EventStoreException
    {
        public EventStoreAggregateNotFoundException()
        {
        }

        public EventStoreAggregateNotFoundException(string message) : base(message)
        {
        }

        public EventStoreAggregateNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventStoreAggregateNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


    [Serializable]
    public class EventStoreCommunicationException : EventStoreException
    {
        public EventStoreCommunicationException()
        {
        }

        public EventStoreCommunicationException(string message) : base(message)
        {
        }

        public EventStoreCommunicationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventStoreCommunicationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}