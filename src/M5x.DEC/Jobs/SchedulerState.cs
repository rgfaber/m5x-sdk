using System.Collections.Immutable;
using M5x.DEC.Jobs.Commands;

namespace M5x.DEC.Jobs
{
    public class SchedulerState<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public SchedulerState(
            ImmutableDictionary<TIdentity, Schedule<TJob, TIdentity>> entries)
        {
            Entries = entries;
        }

        public static SchedulerState<TJob, TIdentity> New { get; } =
            new(ImmutableDictionary<TIdentity, Schedule<TJob, TIdentity>>.Empty);

        public ImmutableDictionary<TIdentity, Schedule<TJob, TIdentity>> Entries { get; }

        public SchedulerState<TJob, TIdentity> AddEntry(Schedule<TJob, TIdentity> entry)
        {
            return new SchedulerState<TJob, TIdentity>(Entries.SetItem(entry.JobId, entry));
        }

        public SchedulerState<TJob, TIdentity> RemoveEntry(TIdentity jobId)
        {
            return new SchedulerState<TJob, TIdentity>(Entries.Remove(jobId));
        }
    }
}