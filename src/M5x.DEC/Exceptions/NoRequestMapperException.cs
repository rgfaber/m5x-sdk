using System;
using System.Runtime.Serialization;

namespace M5x.DEC.Exceptions;

public class NoRequestMapperException : Exception
{
    public NoRequestMapperException()
    {
    }

    protected NoRequestMapperException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NoRequestMapperException(string? message) : base(message)
    {
    }

    public NoRequestMapperException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}