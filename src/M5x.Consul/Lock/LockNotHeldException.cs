using System;

namespace M5x.Consul.Lock
{
    public class LockNotHeldException : Exception
    {
        public LockNotHeldException()
        {
        }

        public LockNotHeldException(string message)
            : base(message)
        {
        }

        public LockNotHeldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}