using System;
using System.Runtime.Serialization;
using System.Text;

namespace M5x.DEC.Persistence.EventStore;

[Serializable]
public class EventStoreDeserializationException : EventStoreException
{
    public EventStoreDeserializationException(string message, byte[] data, Exception inner) : base(message, inner)
    {
        Target = Encoding.Default.GetString(data);
    }

    public EventStoreDeserializationException()
    {
    }

    public EventStoreDeserializationException(string message) : base(message)
    {
    }

    public EventStoreDeserializationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected EventStoreDeserializationException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }

    public string Target { get; }
}