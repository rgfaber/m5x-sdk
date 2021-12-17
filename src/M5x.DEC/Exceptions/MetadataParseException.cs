using System;

namespace M5x.DEC.Exceptions;

public class MetadataParseException : Exception
{
    public MetadataParseException(string key, string value, Exception innerException)
        : base($"Failed to parse metadata key '{key}' with value '{value}' due to '{innerException.Message}'",
            innerException)
    {
    }
}