using System;

namespace M5x.DEC.Jobs.Commands;

public sealed record ScheduleRepeatedly<TJob, TIdentity> : Schedule<TJob, TIdentity>
    where TJob : IJob
    where TIdentity : IJobId
{
    public ScheduleRepeatedly(
        TIdentity jobId,
        TJob job,
        TimeSpan interval,
        DateTime triggerDate,
        object ack = null,
        object nack = null)
        : base(jobId, job, triggerDate, ack, nack)
    {
        if (interval == default) throw new ArgumentException(nameof(interval));

        Interval = interval;
    }

    public TimeSpan Interval { get; }

    public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
    {
        return new ScheduleRepeatedly<TJob, TIdentity>(JobId, Job, Interval, TriggerDate + Interval);
    }

    public override Schedule<TJob, TIdentity> WithAck(object ack)
    {
        return new ScheduleRepeatedly<TJob, TIdentity>(JobId, Job, Interval, TriggerDate, ack, Nack);
    }

    public override Schedule<TJob, TIdentity> WithNack(object nack)
    {
        return new ScheduleRepeatedly<TJob, TIdentity>(JobId, Job, Interval, TriggerDate, Ack, nack);
    }
}