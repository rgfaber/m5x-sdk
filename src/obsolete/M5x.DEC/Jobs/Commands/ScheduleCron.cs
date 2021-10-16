using System;
using Cronos;

namespace M5x.DEC.Jobs.Commands
{
    public sealed record ScheduleCron<TJob, TIdentity> : Schedule<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        private readonly CronExpression _expression;

        public ScheduleCron(
            TIdentity jobId,
            TJob job,
            string cronExpression,
            DateTime triggerDate,
            object ack = null,
            object nack = null)
            : base(jobId, job, triggerDate, ack, nack)
        {
            if (string.IsNullOrWhiteSpace(cronExpression)) throw new ArgumentNullException(nameof(cronExpression));

            CronExpression = cronExpression;
            _expression = Cronos.CronExpression.Parse(cronExpression);
        }

        public string CronExpression { get; }

        public override Schedule<TJob, TIdentity> WithNextTriggerDate(DateTime utcDate)
        {
            var next = _expression.GetNextOccurrence(utcDate);
            if (next.HasValue)
                return new ScheduleCron<TJob, TIdentity>(JobId, Job, CronExpression, next.Value);

            return null;
        }

        public override Schedule<TJob, TIdentity> WithAck(object ack)
        {
            return new ScheduleCron<TJob, TIdentity>(JobId, Job, CronExpression, TriggerDate, ack, Nack);
        }

        public override Schedule<TJob, TIdentity> WithNack(object nack)
        {
            return new ScheduleCron<TJob, TIdentity>(JobId, Job, CronExpression, TriggerDate, Ack, nack);
        }
    }
}