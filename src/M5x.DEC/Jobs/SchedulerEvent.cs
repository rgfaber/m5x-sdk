using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Jobs
{
    public abstract class SchedulerEvent<TJob, TIdentity> : ISchedulerEvent<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public override string ToString()
        {
            return $"{typeof(TJob).PrettyPrint()}/{GetType().PrettyPrint()}";
        }
    }
}