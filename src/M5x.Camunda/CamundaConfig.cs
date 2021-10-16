using System;

namespace M5x.Camunda
{
    public class CamundaConfig
    {
        public static CamundaConfig Default = new();

        public static string EngineUrl =>
            Environment.GetEnvironmentVariable(EnVars.CAMUNDA_URL) ?? "http://localhost:8080/engine-rest";

        public string BasicAuthorizationKey
        {
            get => Environment.GetEnvironmentVariable(EnVars.CAMUNDA_AUTH_KEY) ?? "ZGVtbzpkZW1v";
            set => Environment.SetEnvironmentVariable(EnVars.CAMUNDA_AUTH_KEY, value);
        }
    }
}