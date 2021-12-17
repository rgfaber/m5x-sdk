using System;

namespace M5x.Consul.Semaphore;

public class SemaphoreLimitConflictException : Exception
{
    public SemaphoreLimitConflictException()
    {
    }

    public SemaphoreLimitConflictException(string message, int remoteLimit, int localLimit)
        : base(message)
    {
        RemoteLimit = remoteLimit;
        LocalLimit = localLimit;
    }

    public SemaphoreLimitConflictException(string message, int remoteLimit, int localLimit, Exception inner)
        : base(message, inner)
    {
        RemoteLimit = remoteLimit;
        LocalLimit = localLimit;
    }

    public int RemoteLimit { get; }
    public int LocalLimit { get; }
}