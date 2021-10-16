using System;
using M5x.Schemas.ValueObjects;

namespace M5x.DEC.Jobs
{
    public abstract record SchedulerMessage<TJob, TIdentity> : ValueObject
        where TJob : IJob
        where TIdentity : IJobId
    {
    }

    public abstract record SchedulerCommand<TJob, TIdentity> : SchedulerMessage<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public SchedulerCommand(
            TIdentity jobId,
            object ack = null,
            object nack = null)
        {
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));

            JobId = jobId;
            Ack = ack;
            Nack = nack;
        }

        public TIdentity JobId { get; }
        public object Ack { get; }
        public object Nack { get; }
    }
}