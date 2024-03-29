﻿using System;
using System.Runtime.Serialization;

namespace M5x.Consul.Client;

/// <summary>
///     Represents errors that occur during initalization of the Consul client's configuration.
/// </summary>
#if !(CORECLR || PORTABLE || PORTABLE40)
[Serializable]
#endif
public class ConsulConfigurationException : Exception
{
    public ConsulConfigurationException()
    {
    }

    public ConsulConfigurationException(string message) : base(message)
    {
    }

    public ConsulConfigurationException(string message, Exception inner) : base(message, inner)
    {
    }
#if !(CORECLR || PORTABLE || PORTABLE40)
    protected ConsulConfigurationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
#endif
}