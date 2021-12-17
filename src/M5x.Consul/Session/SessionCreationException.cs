using System;

namespace M5x.Consul.Session;

public class SessionCreationException : Exception
{
    public SessionCreationException()
    {
    }

    public SessionCreationException(string message) : base(message)
    {
    }

    public SessionCreationException(string message, Exception inner) : base(message, inner)
    {
    }
}