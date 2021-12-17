using System;
using M5x.DEC.Jobs.Commands;

namespace M5x.DEC.Jobs.Events;

public class Scheduled<TJob, TIdentity> : SchedulerEvent<TJob, TIdentity>
    where TJob : IJob
    where TIdentity : IJobId
{
    public Scheduled(
        Schedule<TJob, TIdentity> entry)
    {
        if (entry == null) throw new ArgumentException(nameof(entry));

        Entry = entry;
    }

    public Schedule<TJob, TIdentity> Entry { get; }
}