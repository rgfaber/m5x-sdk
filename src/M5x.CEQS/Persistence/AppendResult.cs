namespace M5x.CEQS.Persistence
{
    public class AppendResult
    {
        public AppendResult(long nextExpectedVersion)
        {
            NextExpectedVersion = nextExpectedVersion;
        }

        public long NextExpectedVersion { get; }
    }
}