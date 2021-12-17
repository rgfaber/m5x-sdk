using M5x.DEC.Schema.Common;

namespace M5x.DEC.Schema;

public interface IAck<TPayload> : IFact<TPayload>
    where TPayload : IPayload
{
    string Receiver { get; set; }
}

public abstract record Ack<TPayload> : Fact<TPayload>, IAck<TPayload>
    where TPayload : IPayload
{
    protected Ack(AggregateInfo meta, string correlationId, TPayload payload) : base(meta, correlationId, payload)
    {
    }

    protected Ack()
    {
    }

    public string Receiver { get; set; }
}

public record DefaultAck : Ack<Dummy>
{
    public DefaultAck(AggregateInfo meta, string correlationId, Dummy payload)
        : base(meta, correlationId, payload)
    {
    }

    public DefaultAck()
    {
    }

    public static DefaultAck FromFact<TFact>(string receiver, TFact fact) where TFact : IFact
    {
        return new DefaultAck
        {
            Meta = fact.Meta,
            Receiver = receiver,
            CorrelationId = fact.CorrelationId
        };
    }
}