using System;
using System.Net;

namespace M5x.Chassis;

internal class ApiConfig
{
    public static string[] CorsDomains = Environment.GetEnvironmentVariable(EnVars.CorsDomains)?.Split(',');
    public static bool IsReverse => Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.IsReverse));

    public static bool AllowCors =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.AllowCors) ?? "true");

    public static bool UseHttps =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.UseHttps) ?? "false");

    public static bool UseMvc => Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.UseMvc) ?? "true");


    internal static int GetApiPort()
    {
        var p = Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.ServicePort));
        return p == 0 ? NetUtils.GetFirstUnusedPort(NetUtils.GetLocalIpAddress()) : p;
    }


    public static IPAddress GetApiAddress()
    {
        var p = Environment.GetEnvironmentVariable(EnVars.ServiceIpAddress) ?? "0.0.0.0";
        return NetUtils.GetLocalIpAddress();
    }
}