using System;

namespace M5x.Config
{
    public static class GlobalConfig
    {
        public static bool IsTest => Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.M5_IS_TEST));
    }

    public static class EnVars
    {
        public const string M5_IS_TEST = "M5_IS_TEST";
    }
}