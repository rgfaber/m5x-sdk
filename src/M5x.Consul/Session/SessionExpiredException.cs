using System;

namespace M5x.Consul.Session
{
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException()
        {
        }

        public SessionExpiredException(string message) : base(message)
        {
        }

        public SessionExpiredException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}