using System.Text.Json;

namespace M5x.DEC.Schema;
// public interface IAggregateInfo : IPayload
// {
//     public string Id { get;  }
//     public long Version { get; }
//     public Enum Status { get;  }
//     IAggregateInfo Empty => default;
//
// }

public record AggregateInfo // : IAggregateInfo
{
    public static readonly AggregateInfo Empty = new();

    public AggregateInfo()
    {
    }

    private AggregateInfo(string id, long version, int status)
    {
        Id = id;
        Status = status;
        Version = version;
    }

    public string Id { get; set; }
    public long Version { get; set; }
    public int Status { get; set; }

    public static AggregateInfo New(string id, long version = -1, int status = 0)
    {
        return new AggregateInfo(id, version, status);
    }


    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}