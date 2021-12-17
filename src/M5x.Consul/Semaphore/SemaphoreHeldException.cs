using System;

namespace M5x.Consul.Semaphore;

public class SemaphoreHeldException : Exception
{
    public SemaphoreHeldException()
    {
    }

    public SemaphoreHeldException(string message)
        : base(message)
    {
    }

    public SemaphoreHeldException(string message, Exception inner)
        : base(message, inner)
    {
    }
}