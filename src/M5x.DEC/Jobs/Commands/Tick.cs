namespace M5x.DEC.Jobs.Commands
{
    public sealed record Tick<TJob, TIdentity> : SchedulerMessage<TJob, TIdentity>
        where TJob : IJob
        where TIdentity : IJobId
    {
        private Tick()
        {
        }

        public static Tick<TJob, TIdentity> Instance { get; } = new();
    }
}