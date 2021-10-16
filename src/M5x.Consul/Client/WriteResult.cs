namespace M5x.Consul.Client
{
    /// <summary>
    ///     The result of a Consul API write
    /// </summary>
    public class WriteResult : ConsulResult
    {
        public WriteResult()
        {
        }

        public WriteResult(WriteResult other) : base(other)
        {
        }
    }
}