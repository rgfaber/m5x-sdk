namespace M5x.DEC.Jobs.Commands;

public record Cancel<TJob, TIdentity> : SchedulerCommand<TJob, TIdentity>
    where TJob : IJob
    where TIdentity : IJobId
{
    public Cancel(
        TIdentity jobId,
        object ack = null,
        object nack = null)
        : base(jobId, ack, nack)
    {
    }

    public virtual Cancel<TJob, TIdentity> WithAck(object ack)
    {
        return new Cancel<TJob, TIdentity>(JobId, ack, Nack);
    }

    public virtual Cancel<TJob, TIdentity> WithNack(object nack)
    {
        return new Cancel<TJob, TIdentity>(JobId, Ack, nack);
    }
}