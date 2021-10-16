using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;
using M5x.Consul.KV;
using M5x.Consul.Session;
using M5x.Consul.Utilities;

namespace M5x.Consul.Lock
{
    /// <summary>
    ///     Lock is used to implement client-side leader election. It is follows the algorithm as described here:
    ///     https://consul.io/docs/guides/leader-election.html.
    /// </summary>
    public class Lock : IDistributedLock
    {
        /// <summary>
        ///     LockFlagValue is a magic flag we set to indicate a key is being used for a lock. It is used to detect a potential
        ///     conflict with a semaphore.
        /// </summary>
        private const ulong LockFlagValue = 0x2ddccbc058a50c18;

        /// <summary>
        ///     DefaultLockWaitTime is how long we block for at a time to check if lock acquisition is possible. This affects the
        ///     minimum time it takes to cancel a Lock acquisition.
        /// </summary>
        public static readonly TimeSpan DefaultLockWaitTime = TimeSpan.FromSeconds(15);

        /// <summary>
        ///     DefaultLockRetryTime is how long we wait after a failed lock acquisition before attempting
        ///     to do the lock again. This is so that once a lock-delay is in effect, we do not hot loop
        ///     retrying the acquisition.
        /// </summary>
        public static readonly TimeSpan DefaultLockRetryTime = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     DefaultMonitorRetryTime is how long we wait after a failed monitor check
        ///     of a lock (500 response code). This allows the monitor to ride out brief
        ///     periods of unavailability, subject to the MonitorRetries setting in the
        ///     lock options which is by default set to 0, disabling this feature.
        /// </summary>
        public static readonly TimeSpan DefaultMonitorRetryTime = TimeSpan.FromSeconds(2);

        private readonly ConsulClient.ConsulClient _client;

        private readonly AsyncLock _mutex = new();

        private CancellationTokenSource _cts;
        private Task _monitorTask;
        private int _retries;
        private Task _sessionRenewTask;

        internal Lock(ConsulClient.ConsulClient c)
        {
            _client = c;
            _cts = new CancellationTokenSource();
        }

        public LockOptions Opts { get; set; }
        public string LockSession { get; set; }

        /// <summary>
        ///     If the lock is held or not.
        ///     Users of the Lock object should check the IsHeld property before entering the critical section of their code, e.g.
        ///     in a "while (myLock.IsHeld) {criticalsection}" block.
        ///     Calls to IsHeld are syncronized across threads using a lock, so multiple threads sharing a single Consul Lock will
        ///     queue up reading the IsHeld property of the lock.
        /// </summary>
        public bool IsHeld { get; private set; }

        /// <summary>
        ///     Lock attempts to acquire the lock and blocks while doing so.
        ///     Providing a CancellationToken can be used to abort the lock attempt.
        ///     There is no notification that the lock has been lost, but IsHeld may be set to False at any time due to session
        ///     invalidation, communication errors, operator intervention, etc.
        ///     It is NOT safe to assume that the lock is held until Unlock() unless the Session is specifically created without
        ///     any associated health checks.
        ///     Users of the Lock object should check the IsHeld property before entering the critical section of their code, e.g.
        ///     in a "while (myLock.IsHeld) {criticalsection}" block.
        ///     By default Consul sessions prefer liveness over safety and an application must be able to handle the lock being
        ///     lost.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel lock acquisition</param>
        public async Task<CancellationToken> Acquire(CancellationToken ct)
        {
            try
            {
                using (await _mutex.LockAsync().ConfigureAwait(false))
                {
                    if (IsHeld) throw new LockHeldException();

                    // Don't overwrite the CancellationTokenSource until AFTER we've tested for holding,
                    // since there might be tasks that are currently running for this lock.
                    DisposeCancellationTokenSource();
                    _cts = new CancellationTokenSource();

                    // Check if we need to create a session first
                    if (string.IsNullOrEmpty(Opts.Session))
                    {
                        LockSession = await CreateSession().ConfigureAwait(false);
                        _sessionRenewTask = _client.Session.RenewPeriodic(Opts.SessionTtl, LockSession,
                            WriteOptions.Default, _cts.Token);
                    }
                    else
                    {
                        LockSession = Opts.Session;
                    }

                    var qOpts = new QueryOptions
                    {
                        WaitTime = Opts.LockWaitTime
                    };

                    var attempts = 0;
                    var start = DateTime.UtcNow;

                    while (!ct.IsCancellationRequested)
                    {
                        if (attempts > 0 && Opts.LockTryOnce)
                        {
                            var elapsed = DateTime.UtcNow.Subtract(start);
                            if (elapsed > qOpts.WaitTime)
                            {
                                DisposeCancellationTokenSource();
                                throw new LockMaxAttemptsReachedException(
                                    "LockTryOnce is set and the lock is already held or lock delay is in effect");
                            }

                            qOpts.WaitTime -= elapsed;
                        }

                        attempts++;

                        QueryResult<KVPair> pair;

                        pair = await _client.KV.Get(Opts.Key, qOpts).ConfigureAwait(false);

                        if (pair.Response != null)
                        {
                            if (pair.Response.Flags != LockFlagValue)
                            {
                                DisposeCancellationTokenSource();
                                throw new LockConflictException();
                            }

                            // Already locked by this session
                            if (pair.Response.Session == LockSession)
                            {
                                // Don't restart MonitorLock if this session already holds the lock
                                if (IsHeld) return _cts.Token;
                                IsHeld = true;
                                _monitorTask = MonitorLock();
                                return _cts.Token;
                            }

                            // If it's not empty, some other session must have the lock
                            if (!string.IsNullOrEmpty(pair.Response.Session))
                            {
                                qOpts.WaitIndex = pair.LastIndex;
                                continue;
                            }
                        }

                        // If the code executes this far, no other session has the lock, so try to lock it
                        var kvPair = LockEntry(LockSession);
                        var locked = (await _client.KV.Acquire(kvPair).ConfigureAwait(false)).Response;

                        // KV acquisition succeeded, so the session now holds the lock
                        if (locked)
                        {
                            IsHeld = true;
                            _monitorTask = MonitorLock();
                            return _cts.Token;
                        }

                        // Handle the case of not getting the lock
                        if (ct.IsCancellationRequested)
                        {
                            DisposeCancellationTokenSource();
                            throw new TaskCanceledException();
                        }

                        // Failed to get the lock, determine why by querying for the key again
                        qOpts.WaitIndex = 0;
                        pair = await _client.KV.Get(Opts.Key, qOpts).ConfigureAwait(false);

                        // If the session is not null, this means that a wait can safely happen using a long poll
                        if (pair.Response != null && pair.Response.Session != null)
                        {
                            qOpts.WaitIndex = pair.LastIndex;
                            continue;
                        }

                        // If the session is null and the lock failed to acquire, then it means
                        // a lock-delay is in effect and a timed wait must be used to avoid a hot loop.
                        try
                        {
                            await Task.Delay(Opts.LockRetryTime, ct).ConfigureAwait(false);
                        }
                        catch (TaskCanceledException)
                        {
                            /* Ignore TaskTaskCanceledException */
                        }
                    }

                    DisposeCancellationTokenSource();
                    throw new LockNotHeldException("Unable to acquire the lock with Consul");
                }
            }
            finally
            {
                if (ct.IsCancellationRequested || !IsHeld && !string.IsNullOrEmpty(Opts.Session))
                {
                    DisposeCancellationTokenSource();
                    if (_sessionRenewTask != null)
                        try
                        {
                            await _monitorTask.ConfigureAwait(false);
                            await _sessionRenewTask.ConfigureAwait(false);
                        }
                        catch (AggregateException)
                        {
                            // Ignore AggregateExceptions from the tasks during Release, since if the Renew task died, the developer will be Super Confused if they see the exception during Release.
                        }
                }
            }
        }

        /// <summary>
        ///     Unlock released the lock. It is an error to call this if the lock is not currently held.
        /// </summary>
        public async Task Release(CancellationToken ct = default)
        {
            try
            {
                using (await _mutex.LockAsync().ConfigureAwait(false))
                {
                    if (!IsHeld) throw new LockNotHeldException();
                    IsHeld = false;

                    DisposeCancellationTokenSource();

                    var lockEnt = LockEntry(LockSession);

                    await _client.KV.Release(lockEnt, ct).ConfigureAwait(false);
                }
            }
            finally
            {
                if (_sessionRenewTask != null)
                    try
                    {
                        await _sessionRenewTask.ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        // Ignore Exceptions from the tasks during Release, since if the Renew task died, the developer will be Super Confused if they see the exception during Release.
                    }
            }
        }

        /// <summary>
        ///     Destroy is used to cleanup the lock entry. It is not necessary to invoke. It will fail if the lock is in use.
        /// </summary>
        public async Task Destroy(CancellationToken ct = default)
        {
            using (await _mutex.LockAsync().ConfigureAwait(false))
            {
                if (IsHeld) throw new LockHeldException();

                var pair = (await _client.KV.Get(Opts.Key, ct).ConfigureAwait(false)).Response;

                if (pair == null) return;

                if (pair.Flags != LockFlagValue) throw new LockConflictException();

                if (!string.IsNullOrEmpty(pair.Session)) throw new LockInUseException();

                var didRemove = (await _client.KV.DeleteCAS(pair, ct).ConfigureAwait(false)).Response;

                if (!didRemove) throw new LockInUseException();
            }
        }

        /// <summary>
        ///     Lock attempts to acquire the lock and blocks while doing so. Not providing a CancellationToken means the thread can
        ///     block indefinitely until the lock is acquired.
        ///     There is no notification that the lock has been lost, but it may be closed at any time due to session invalidation,
        ///     communication errors, operator intervention, etc.
        ///     It is NOT safe to assume that the lock is held until Unlock() unless the Session is specifically created without
        ///     any associated health checks.
        ///     Users of the Lock object should check the IsHeld property before entering the critical section of their code, e.g.
        ///     in a "while (myLock.IsHeld) {criticalsection}" block.
        ///     By default Consul sessions prefer liveness over safety and an application must be able to handle the lock being
        ///     lost.
        /// </summary>
        public Task<CancellationToken> Acquire()
        {
            return Acquire(CancellationToken.None);
        }

        private void DisposeCancellationTokenSource()
        {
            // Make a copy of the reference to the CancellationTokenSource in case it gets removed before we finish.
            // It's okay to cancel and dispose of them twice, it doesn't cause exceptions.
            var cts = _cts;
            if (cts != null)
            {
                Interlocked.CompareExchange(ref _cts, null, cts);
                cts.Cancel();
                cts.Dispose();
            }
        }

        /// <summary>
        ///     MonitorLock is a long running routine to monitor a lock ownership. It sets IsHeld to false if we lose our
        ///     leadership.
        /// </summary>
        private Task MonitorLock()
        {
            return Task.Factory.StartNew(async () =>
            {
                // Copy a reference to _cts since we could end up destroying it before this method returns
                var cts = _cts;
                try
                {
                    var opts = new QueryOptions { Consistency = ConsistencyMode.Consistent };
                    _retries = Opts.MonitorRetries;
                    while (IsHeld && !cts.Token.IsCancellationRequested)
                        try
                        {
                            // Check to see if the current session holds the lock
                            var pair = await _client.KV.Get(Opts.Key, opts).ConfigureAwait(false);
                            if (pair.Response != null)
                            {
                                _retries = Opts.MonitorRetries;

                                // Lock is no longer held! Shut down everything.
                                if (pair.Response.Session != LockSession)
                                {
                                    IsHeld = false;
                                    DisposeCancellationTokenSource();
                                    return;
                                }

                                // Lock is still held, start a blocking query
                                opts.WaitIndex = pair.LastIndex;
                            }
                            else
                            {
                                // Failsafe in case the KV store is unavailable
                                IsHeld = false;
                                DisposeCancellationTokenSource();
                                return;
                            }
                        }
                        catch (ConsulRequestException)
                        {
                            if (_retries > 0)
                            {
                                await Task.Delay(Opts.MonitorRetryTime, cts.Token).ConfigureAwait(false);
                                _retries--;
                                opts.WaitIndex = 0;
                                continue;
                            }

                            throw;
                        }
                        catch (OperationCanceledException)
                        {
                            // Ignore and retry since this could be the underlying HTTPClient being swapped out/disposed of.
                        }
                }
                finally
                {
                    IsHeld = false;
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        /// <summary>
        ///     CreateSession is used to create a new managed session
        /// </summary>
        /// <returns>The session ID</returns>
        private async Task<string> CreateSession()
        {
            var se = new SessionEntry
            {
                Name = Opts.SessionName,
                Ttl = Opts.SessionTtl
            };
            return (await _client.Session.Create(se).ConfigureAwait(false)).Response;
        }

        /// <summary>
        ///     LockEntry returns a formatted KVPair for the lock
        /// </summary>
        /// <param name="session">The session ID</param>
        /// <returns>A KVPair with the lock flag set</returns>
        private KVPair LockEntry(string session)
        {
            return new KVPair(Opts.Key)
            {
                Value = Opts.Value,
                Session = session,
                Flags = LockFlagValue
            };
        }
    }
}