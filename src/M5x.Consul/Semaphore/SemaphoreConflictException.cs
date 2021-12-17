using System;

namespace M5x.Consul.Semaphore;

public class SemaphoreConflictException : Exception
{
    public SemaphoreConflictException()
    {
    }

    public SemaphoreConflictException(string message)
        : base(message)
    {
    }

    public SemaphoreConflictException(string message, Exception inner)
        : base(message, inner)
    {
    }
}