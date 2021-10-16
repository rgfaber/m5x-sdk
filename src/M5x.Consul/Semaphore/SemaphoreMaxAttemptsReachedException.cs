using System;

namespace M5x.Consul.Semaphore
{
    public class SemaphoreMaxAttemptsReachedException : Exception
    {
        public SemaphoreMaxAttemptsReachedException()
        {
        }

        public SemaphoreMaxAttemptsReachedException(string message) : base(message)
        {
        }

        public SemaphoreMaxAttemptsReachedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}