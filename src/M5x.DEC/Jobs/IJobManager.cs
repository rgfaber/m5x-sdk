namespace M5x.DEC.Jobs;

public interface IJobManager
{
}

public interface IJobManager<TJob, TIdentity> : IJobManager
    where TJob : IJob
    where TIdentity : IJobId
{
}