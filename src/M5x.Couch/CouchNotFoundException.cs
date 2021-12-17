using System;

namespace M5x.Couch;

/// <summary>
///     Represents a HttpStatusCode of 404, document not found.
/// </summary>
public class CouchNotFoundException : Exception
{
    public CouchNotFoundException(string msg, Exception e) : base(msg, e)
    {
    }
}