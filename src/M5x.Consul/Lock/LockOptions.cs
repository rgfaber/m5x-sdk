using System;
using M5x.Consul.Utilities;

namespace M5x.Consul.Lock;

/// <summary>
///     LockOptions is used to parameterize the Lock behavior.
/// </summary>
public class LockOptions
{
    /// <summary>
    ///     DefaultLockSessionName is the Session Name we assign if none is provided
    /// </summary>
    private const string DefaultLockSessionName = "Consul API Lock";

    private static readonly TimeSpan LockRetryTimeMin = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     DefaultLockSessionTTL is the default session TTL if no Session is provided when creating a new Lock. This is used
    ///     because we do not have another other check to depend upon.
    /// </summary>
    private static readonly TimeSpan DefaultLockSessionTtl = TimeSpan.FromSeconds(15);

    private TimeSpan _lockRetryTime;

    public LockOptions(string key)
    {
        Key = key;
        SessionName = DefaultLockSessionName;
        SessionTtl = DefaultLockSessionTtl;
        MonitorRetryTime = Lock.DefaultMonitorRetryTime;
        LockWaitTime = Lock.DefaultLockWaitTime;
        LockRetryTime = Lock.DefaultLockRetryTime;
    }

    public string Key { get; set; }
    public byte[] Value { get; set; }
    public string Session { get; set; }
    public string SessionName { get; set; }
    public TimeSpan SessionTtl { get; set; }
    public int MonitorRetries { get; set; }

    public TimeSpan LockRetryTime
    {
        get => _lockRetryTime;
        set
        {
            if (value < LockRetryTimeMin)
                throw new ArgumentOutOfRangeException(nameof(LockRetryTime),
                    $"The retry time must be greater than {LockRetryTimeMin.ToGoDuration()}.");

            _lockRetryTime = value;
        }
    }

    public TimeSpan LockWaitTime { get; set; }
    public TimeSpan MonitorRetryTime { get; set; }
    public bool LockTryOnce { get; set; }
}