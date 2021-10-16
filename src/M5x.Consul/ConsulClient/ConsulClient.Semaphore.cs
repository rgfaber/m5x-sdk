using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Interfaces;
using M5x.Consul.Lock;
using M5x.Consul.Semaphore;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        /// <summary>
        ///     Used to created a Semaphore which will operate at the given KV prefix and uses the given limit for the semaphore.
        ///     The prefix must have write privileges, and the limit must be agreed upon by all contenders.
        /// </summary>
        /// <param name="prefix">The keyspace prefix (e.g. "locks/semaphore")</param>
        /// <param name="limit">The number of available semaphore slots</param>
        /// <returns>An unlocked semaphore</returns>
        public IDistributedSemaphore Semaphore(string prefix, int limit)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));
            return Semaphore(new SemaphoreOptions(prefix, limit));
        }

        /// <summary>
        ///     SemaphoreOpts is used to create a Semaphore with the given options.
        ///     The prefix must have write privileges, and the limit must be agreed upon by all contenders.
        ///     If a Session is not provided, one will be created.
        /// </summary>
        /// <param name="opts">The semaphore options</param>
        /// <returns>An unlocked semaphore</returns>
        public IDistributedSemaphore Semaphore(SemaphoreOptions opts)
        {
            if (opts == null) throw new ArgumentNullException(nameof(opts));
            return new Semaphore.Semaphore(this) { Opts = opts };
        }

        public Task<IDistributedSemaphore> AcquireSemaphore(string prefix, int limit,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            if (limit <= 0) throw new ArgumentNullException(nameof(limit));
            return AcquireSemaphore(new SemaphoreOptions(prefix, limit), ct);
        }

        public async Task<IDistributedSemaphore> AcquireSemaphore(SemaphoreOptions opts,
            CancellationToken ct = default)
        {
            if (opts == null) throw new ArgumentNullException(nameof(opts));

            var semaphore = Semaphore(opts);
            await semaphore.Acquire(ct).ConfigureAwait(false);
            return semaphore;
        }

        public Task ExecuteInSemaphore(string prefix, int limit, Action a,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentNullException(nameof(prefix));
            if (limit <= 0) throw new ArgumentNullException(nameof(limit));
            return ExecuteInSemaphore(new SemaphoreOptions(prefix, limit), a, ct);
        }

        public async Task ExecuteInSemaphore(SemaphoreOptions opts, Action a,
            CancellationToken ct = default)
        {
            if (opts == null) throw new ArgumentNullException(nameof(opts));
            if (a == null) throw new ArgumentNullException(nameof(a));

            var semaphore = await AcquireSemaphore(opts, ct).ConfigureAwait(false);

            try
            {
                if (!semaphore.IsHeld) throw new LockNotHeldException("Could not obtain the lock");
                a();
            }
            finally
            {
                await semaphore.Release(ct).ConfigureAwait(false);
            }
        }
    }
}