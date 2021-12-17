using System;

namespace M5x.Sso;

public static class SsoConfig
{
    public static string SsoUrl => Environment.GetEnvironmentVariable(EnVars.SSO_URL) ?? "http://localhost:8080";
}