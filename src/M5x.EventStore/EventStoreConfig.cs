using System;
using M5x.Config;

namespace M5x.EventStore;

public static class EventStoreConfig
{
    public static string Uri
        => DotEnv.Get(EnVars.EVENTSTORE_URI) ?? Defaults.Uri;

    public static string UserName
        => Environment.GetEnvironmentVariable(EnVars.EVENTSTORE_USER) ?? Defaults.UserName;

    public static string Password
        => Environment.GetEnvironmentVariable(EnVars.EVENTSTORE_PWD) ?? Defaults.Password;

    /// <summary>
    ///     If ture, make sure you have GRPC_DEFAULT_SSL_ROOTS_FILE_PATH environment variable set
    /// </summary>
    public static bool UseTls
        => Convert.ToBoolean(DotEnv.Get(EnVars.EVENTSTORE_USE_TLS) ?? Defaults.False);

    public static bool Insecure => Convert.ToBoolean(DotEnv.Get(EnVars.EVENTSTORE_INSECURE) ?? Defaults.True);

    private static class Defaults
    {
        public const string Uri = "esdb+discover://127.0.0.1:2113?tls=false";

        // public const string Uri = "tcp://es.local:2113";
        // public const string Uri = "esdb://es.local:2113?tls=false";
        public const string UserName = "admin";
        public const string Password = "changeit";
        public static readonly string False = "false";
        public static readonly string True = "true";
    }
}