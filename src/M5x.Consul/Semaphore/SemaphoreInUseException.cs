using System;

namespace M5x.Consul.Semaphore
{
    public class SemaphoreInUseException : Exception
    {
        public SemaphoreInUseException()
        {
        }

        public SemaphoreInUseException(string message)
            : base(message)
        {
        }

        public SemaphoreInUseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}