using System;

namespace M5x.Consul.Semaphore
{
    /// <summary>
    ///     SemaphoreOptions is used to parameterize the Semaphore
    /// </summary>
    public class SemaphoreOptions
    {
        /// <summary>
        ///     DefaultSemaphoreSessionName is the Session Name we assign if none is provided
        /// </summary>
        private const string DefaultLockSessionName = "Consul API Semaphore";

        /// <summary>
        ///     DefaultSemaphoreSessionTTL is the default session TTL if no Session is provided when creating a new Semaphore. This
        ///     is used because we do not have any other check to depend upon.
        /// </summary>
        private readonly TimeSpan _defaultLockSessionTtl = TimeSpan.FromSeconds(15);

        private int _limit;

        private string _prefix;

        public SemaphoreOptions(string prefix, int limit)
        {
            Prefix = prefix;
            Limit = limit;
            SessionName = DefaultLockSessionName;
            SessionTtl = _defaultLockSessionTtl;
            MonitorRetryTime = Semaphore.DefaultMonitorRetryTime;
            SemaphoreWaitTime = Semaphore.DefaultSemaphoreWaitTime;
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _prefix = value;
                else
                    throw new ArgumentException("Semaphore prefix cannot be null or empty", nameof(Prefix));
            }
        }

        public int Limit
        {
            get => _limit;
            set
            {
                if (value > 0)
                    _limit = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(Limit), "Semaphore limit must be greater than 0");
            }
        }

        public byte[] Value { get; set; }
        public string Session { get; set; }
        public string SessionName { get; set; }
        public TimeSpan SessionTtl { get; set; }
        public int MonitorRetries { get; set; }
        public TimeSpan MonitorRetryTime { get; set; }
        public TimeSpan SemaphoreWaitTime { get; set; }
        public bool SemaphoreTryOnce { get; set; }
    }
}