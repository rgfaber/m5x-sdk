using System;

namespace M5x.Consul;

public static class ConsulConfig
{
    public static string Address =>
        Environment.GetEnvironmentVariable(EnVars.CONSUL_HTTP_ADDRESS) ?? "http://localhost:8500";

    public static string Dc => Environment.GetEnvironmentVariable(EnVars.CONSUL_DC) ?? "dc1";

    public static int RefreshDelayMs =>
        Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.CONSUL_REFRESH_DELAY_MS) ?? "10000");

    public static void SetTestEnvironment()
    {
        Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_ADDRESS, "http://localhost:8500");
    }
}