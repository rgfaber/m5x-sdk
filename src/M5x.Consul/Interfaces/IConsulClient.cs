using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Lock;
using M5x.Consul.Semaphore;

namespace M5x.Consul.Interfaces;

public interface IConsulClient : IDisposable
{
    IAclEndpoint ACL { get; }
    IAgentEndpoint Agent { get; }
    ICatalogEndpoint Catalog { get; }
    IEventEndpoint Event { get; }
    IHealthEndpoint Health { get; }
    IKvEndpoint KV { get; }
    IRawEndpoint Raw { get; }
    ISessionEndpoint Session { get; }
    IStatusEndpoint Status { get; }
    IOperatorEndpoint Operator { get; }
    IPreparedQueryEndpoint PreparedQuery { get; }
    ICoordinateEndpoint Coordinate { get; }
    ISnapshotEndpoint Snapshot { get; }
    Task<IDistributedLock> AcquireLock(LockOptions opts, CancellationToken ct = default);
    Task<IDistributedLock> AcquireLock(string key, CancellationToken ct = default);

    Task<IDistributedSemaphore> AcquireSemaphore(SemaphoreOptions opts,
        CancellationToken ct = default);

    Task<IDistributedSemaphore> AcquireSemaphore(string prefix, int limit,
        CancellationToken ct = default);

    IDistributedLock CreateLock(LockOptions opts);
    IDistributedLock CreateLock(string key);
    Task ExecuteInSemaphore(SemaphoreOptions opts, Action a, CancellationToken ct = default);
    Task ExecuteInSemaphore(string prefix, int limit, Action a, CancellationToken ct = default);
    Task ExecuteLocked(LockOptions opts, Action action, CancellationToken ct = default);

    [Obsolete(
        "This method will be removed in 0.8.0. Replace calls with the method signature ExecuteLocked(LockOptions, Action, CancellationToken)")]
    Task ExecuteLocked(LockOptions opts, CancellationToken ct, Action action);

    Task ExecuteLocked(string key, Action action, CancellationToken ct = default);

    [Obsolete(
        "This method will be removed in 0.8.0. Replace calls with the method signature ExecuteLocked(string, Action, CancellationToken)")]
    Task ExecuteLocked(string key, CancellationToken ct, Action action);

    IDistributedSemaphore Semaphore(SemaphoreOptions opts);
    IDistributedSemaphore Semaphore(string prefix, int limit);
}