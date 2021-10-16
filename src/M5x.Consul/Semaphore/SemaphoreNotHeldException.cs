using System;

namespace M5x.Consul.Semaphore
{
    public class SemaphoreNotHeldException : Exception
    {
        public SemaphoreNotHeldException()
        {
        }

        public SemaphoreNotHeldException(string message)
            : base(message)
        {
        }

        public SemaphoreNotHeldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}