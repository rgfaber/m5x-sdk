using System;

namespace M5x.Consul.Lock
{
    public class LockHeldException : Exception
    {
        public LockHeldException()
        {
        }

        public LockHeldException(string message)
            : base(message)
        {
        }

        public LockHeldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}