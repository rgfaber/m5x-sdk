using System;

namespace M5x.DEC.Jobs.Events;

public class Finished<TJob, TIdentity> : SchedulerEvent<TJob, TIdentity>
    where TJob : IJob
    where TIdentity : IJobId
{
    public Finished(
        TIdentity jobId,
        DateTime triggerDate)
    {
        if (jobId == null) throw new ArgumentNullException(nameof(jobId));
        if (triggerDate == default) throw new ArgumentException(nameof(triggerDate));

        JobId = jobId;
        TriggerDate = triggerDate;
    }

    public TIdentity JobId { get; }
    public DateTime TriggerDate { get; }
}