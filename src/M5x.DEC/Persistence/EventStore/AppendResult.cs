namespace M5x.DEC.Persistence.EventStore
{
    public class AppendResult
    {
        private AppendResult(long nextExpectedVersion)
        {
            NextExpectedVersion = nextExpectedVersion;
        }

        public long NextExpectedVersion { get; }

        public static AppendResult New(long nextExpectedVersion)
        {
            return new AppendResult(nextExpectedVersion);
        }
    }
}