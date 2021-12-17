using System.ComponentModel.DataAnnotations;

namespace M5x.DEC.Schema;

public abstract record Hope : IHope
{
    protected Hope()
    {
    }

    protected Hope(string aggregateId, string correlationId)
    {
        AggregateId = aggregateId;
        CorrelationId = correlationId;
    }

    public string AggregateId { get; set; }
    public string CorrelationId { get; set; }
}

public abstract record Hope<TPayload> : Hope, IHope<TPayload>
    where TPayload : IPayload
{
    protected Hope()
    {
    }

    protected Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
    {
    }

    protected Hope(string aggregateId, string correlationId, TPayload payload) : base(aggregateId, correlationId)
    {
        Payload = payload;
    }

    public TPayload Payload { get; set; }
}

public interface IHope<TPayload> : IHope
    where TPayload : IPayload
{
    TPayload Payload { get; set; }
}

public interface IHope
{
    [Required(AllowEmptyStrings = false)] string AggregateId { get; set; }
    [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
}