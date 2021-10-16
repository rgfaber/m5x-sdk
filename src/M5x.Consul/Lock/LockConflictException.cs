using System;

namespace M5x.Consul.Lock
{
    public class LockConflictException : Exception
    {
        public LockConflictException()
        {
        }

        public LockConflictException(string message)
            : base(message)
        {
        }

        public LockConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}