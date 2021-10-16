using System;

namespace M5x.Nats
{
    public static class BusConfig
    {
        public static int? Port =>
            Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.NATS_PORT) ?? "4222");

        public static string Uri =>
            Environment.GetEnvironmentVariable(EnVars.NATS_URI) ?? "nats";

        public static string Uris =>
            Environment.GetEnvironmentVariable(EnVars.NATS_URIS) ?? "nats:4222";

        public static string Monitor =>
            Environment.GetEnvironmentVariable(EnVars.NATS_MONITOR_URI) ?? "nats:8222";

        public static string Monitors =>
            Environment.GetEnvironmentVariable(EnVars.NATS_MONITOR_URIS) ?? "nats:8222";

        public static string User =>
            Environment.GetEnvironmentVariable(EnVars.NATS_USER) ?? "guest";

        public static string Password =>
            Environment.GetEnvironmentVariable(EnVars.NATS_PWD) ?? "pa55w0rd";

        public static int? TimeOutMs =>
            Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.NATS_TIME_OUT_MS) ?? "10000");

        public static bool AutoRespondToPing =>
            Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.NATS_AUTO_RESPOND) ?? "true");

        public static string Host =>
            Environment.GetEnvironmentVariable(EnVars.NATS_HOST) ?? "nats";

        public static void BuildEnvironment()
        {
            Environment.SetEnvironmentVariable(EnVars.NATS_URI, Uri);
            Environment.SetEnvironmentVariable(EnVars.NATS_URIS, Uris);
            Environment.SetEnvironmentVariable(EnVars.NATS_MONITOR_URI, Monitor);
            Environment.SetEnvironmentVariable(EnVars.NATS_MONITOR_URIS, Monitors);
            Environment.SetEnvironmentVariable(EnVars.NATS_USER, User);
            Environment.SetEnvironmentVariable(EnVars.NATS_PWD, Password);
            Environment.SetEnvironmentVariable(EnVars.NATS_TIME_OUT_MS, Convert.ToString(TimeOutMs));
            Environment.SetEnvironmentVariable(EnVars.NATS_AUTO_RESPOND, Convert.ToString(AutoRespondToPing));
        }
    }
}