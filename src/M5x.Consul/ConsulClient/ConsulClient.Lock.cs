using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Interfaces;
using M5x.Consul.Lock;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    /// <summary>
    ///     CreateLock returns an unlocked lock which can be used to acquire and release the mutex. The key used must have
    ///     write permissions.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IDistributedLock CreateLock(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        return CreateLock(new LockOptions(key));
    }

    /// <summary>
    ///     CreateLock returns an unlocked lock which can be used to acquire and release the mutex. The key used must have
    ///     write permissions.
    /// </summary>
    /// <param name="opts"></param>
    /// <returns></returns>
    public IDistributedLock CreateLock(LockOptions opts)
    {
        if (opts == null) throw new ArgumentNullException(nameof(opts));
        return new Lock.Lock(this) { Opts = opts };
    }

    /// <summary>
    ///     AcquireLock creates a lock that is already acquired when this call returns.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<IDistributedLock> AcquireLock(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        return AcquireLock(new LockOptions(key), ct);
    }

    /// <summary>
    ///     AcquireLock creates a lock that is already acquired when this call returns.
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IDistributedLock> AcquireLock(LockOptions opts,
        CancellationToken ct = default)
    {
        if (opts == null) throw new ArgumentNullException(nameof(opts));

        var l = CreateLock(opts);
        await l.Acquire(ct).ConfigureAwait(false);
        return l;
    }

    /// <summary>
    ///     ExecuteLock accepts a delegate to execute in the context of a lock, releasing the lock when completed.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public Task ExecuteLocked(string key, Action action, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        return ExecuteLocked(new LockOptions(key), action, ct);
    }

    /// <summary>
    ///     ExecuteLock accepts a delegate to execute in the context of a lock, releasing the lock when completed.
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExecuteLocked(LockOptions opts, Action action,
        CancellationToken ct = default)
    {
        if (opts == null) throw new ArgumentNullException(nameof(opts));
        if (action == null) throw new ArgumentNullException(nameof(action));

        var l = await AcquireLock(opts, ct).ConfigureAwait(false);

        try
        {
            if (!l.IsHeld) throw new LockNotHeldException("Could not obtain the lock");
            action();
        }
        finally
        {
            await l.Release().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     ExecuteLock accepts a delegate to execute in the context of a lock, releasing the lock when completed.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="ct"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Obsolete(
        "This method will be removed in 0.8.0. Replace calls with the method signature ExecuteLocked(string, Action, CancellationToken)")]
    public Task ExecuteLocked(string key, CancellationToken ct, Action action)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        if (action == null) throw new ArgumentNullException(nameof(action));
        return ExecuteLocked(new LockOptions(key), action, ct);
    }

    /// <summary>
    ///     ExecuteLock accepts a delegate to execute in the context of a lock, releasing the lock when completed.
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="ct"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Obsolete(
        "This method will be removed in 0.8.0. Replace calls with the method signature ExecuteLocked(LockOptions, Action, CancellationToken)")]
    public Task ExecuteLocked(LockOptions opts, CancellationToken ct, Action action)
    {
        if (opts == null) throw new ArgumentNullException(nameof(opts));
        return ExecuteLocked(opts, action, ct);
    }
}