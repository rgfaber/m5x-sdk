using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace M5x.DEC.Schema;

public abstract record Response : IResponse
{
    protected Response()
    {
        ErrorState = new ErrorState();
    }

    protected Response(string correlationId) : this()
    {
        CorrelationId = correlationId;
    }

    public int AggregateStatus { get; set; }

    [Required] public ErrorState ErrorState { get; }
    [Required(AllowEmptyStrings = false)] public string CorrelationId { get; set; }
    public bool IsSuccess => ErrorState.IsSuccessful;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public interface IResponse
{
    [Required(AllowEmptyStrings = false)] string CorrelationId { get; set; }
    [Required] ErrorState ErrorState { get; }
    bool IsSuccess { get; }
}