using System;

namespace M5x.Consul.Lock
{
    public class LockMaxAttemptsReachedException : Exception
    {
        public LockMaxAttemptsReachedException()
        {
        }

        public LockMaxAttemptsReachedException(string message) : base(message)
        {
        }

        public LockMaxAttemptsReachedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}