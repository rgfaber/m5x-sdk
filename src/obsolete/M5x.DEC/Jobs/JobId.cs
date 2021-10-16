using M5x.Schemas;

namespace M5x.DEC.Jobs
{
    public record JobId : Identity<JobId>, IJobId
    {
        public JobId(string value)
            : base(value)
        {
        }
    }
}