using M5x.DEC.Schema.ValueObjects;

namespace M5x.DEC.Jobs;

public record JobName : SingleValueObject<string>, IJobName
{
    public JobName(string value)
        : base(value)
    {
    }
}