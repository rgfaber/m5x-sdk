using System;

namespace M5x.Consul.Lock
{
    public class LockInUseException : Exception
    {
        public LockInUseException()
        {
        }

        public LockInUseException(string message)
            : base(message)
        {
        }

        public LockInUseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}