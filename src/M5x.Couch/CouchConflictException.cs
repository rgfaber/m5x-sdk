using System;

namespace M5x.Couch;

/// <summary>
///     Represents a CouchDB HTTP 409 conflict.
/// </summary>
public class CouchConflictException : Exception
{
    public CouchConflictException(string msg, Exception e) : base(msg, e)
    {
    }
}