using M5x.Schemas;

namespace M5x.DEC.Jobs
{
    public interface ISchedulerEvent : IVersionedType
    {
    }

    public interface ISchedulerEvent<TJob, TIdentity> : ISchedulerEvent
        where TJob : IJob
        where TIdentity : IJobId
    {
    }
}