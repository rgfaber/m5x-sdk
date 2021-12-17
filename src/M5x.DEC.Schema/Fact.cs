using Newtonsoft.Json;

namespace M5x.DEC.Schema;

public interface IFact
{
    AggregateInfo Meta { get; set; }
    public string CorrelationId { get; set; }
}

public interface IFact<TPayload> : IFact
    where TPayload : IPayload
{
    TPayload Payload { get; set; }
}

public abstract record Fact<TPayload> : IFact<TPayload>
    where TPayload : IPayload
{
    [JsonConstructor]
    protected Fact(AggregateInfo meta, string correlationId, TPayload payload)
    {
        Meta = meta;
        CorrelationId = correlationId;
        Payload = payload;
    }

    protected Fact()
    {
    }

    public AggregateInfo Meta { get; set; }
    public string CorrelationId { get; set; }
    public TPayload Payload { get; set; }
}