using M5x.Schemas.ValueObjects;

namespace M5x.DEC.Jobs
{
    public record JobName : SingleValueObject<string>, IJobName
    {
        public JobName(string value)
            : base(value)
        {
        }
    }
}