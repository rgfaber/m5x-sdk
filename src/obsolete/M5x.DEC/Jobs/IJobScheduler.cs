namespace M5x.DEC.Jobs
{
    public interface IJobScheduler
    {
    }

    public interface IJobScheduler<out TIdentity>
        where TIdentity : IJobId
    {
    }
}