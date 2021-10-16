﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.Consul.Utilities
{
    // Async lock as outlined by Stephen Toub: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
    internal class AsyncLock
    {
        private readonly Task<Releaser> _releaser;
        private readonly SemaphoreSlim _semaphore;

        public AsyncLock()
        {
            _semaphore = new SemaphoreSlim(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted
                ? _releaser
                : wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        internal struct Releaser : IDisposable
        {
            private readonly AsyncLock _mutex;

            internal Releaser(AsyncLock mutex)
            {
                _mutex = mutex;
            }

            public void Dispose()
            {
                if (_mutex != null)
                    _mutex._semaphore.Release();
            }
        }
    }
}