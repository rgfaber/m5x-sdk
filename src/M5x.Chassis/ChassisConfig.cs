using System;

namespace M5x.Chassis
{
    public static class ChassisConfig
    {
        public static bool IsReverse = Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.IsReverse));

        public static bool UseDiscovery = Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.UseDiscovery));
    }
}