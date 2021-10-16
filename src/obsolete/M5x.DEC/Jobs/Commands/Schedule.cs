using System;

namespace M5x.DEC.Jobs.Commands
{
    public record Schedule<TJob, TIdentity> : SchedulerCommand<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        public Schedule(
            TIdentity jobId,
            TJob job,
            DateTime triggerDate,
            object ack = null,
            object nack = null)
            : base(jobId, ack, nack)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            if (triggerDate == default) throw new ArgumentException(nameof(triggerDate));

            Job = job;
            TriggerDate = triggerDate;
        }

        public TJob Job { get; }
        public DateTime TriggerDate { get; }

        public virtual Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            return null;
        }

        public virtual Schedule<TJob, TIdentity> WithAck(object ack)
        {
            return new(JobId, Job, TriggerDate, ack, Nack);
        }

        public virtual Schedule<TJob, TIdentity> WithNack(object nack)
        {
            return new(JobId, Job, TriggerDate, Ack, nack);
        }

        public virtual Schedule<TJob, TIdentity> WithOutAcks()
        {
            return WithAck(null).WithNack(null);
        }
    }
}