using System;
using M5x.Config;

namespace M5x.Redis
{
    public static class Config
    {
        public static string Password => DotEnv.Get(EnVars.REDIS_PWD);
        public static string User => DotEnv.Get(EnVars.REDIS_USER);
        public static bool UseSsl => Convert.ToBoolean(DotEnv.Get(EnVars.REDIS_USE_SSL));

        public static string ConfigString
        {
            get
            {
                var s = DotEnv.Get(EnVars.REDIS_CONFIG) ?? "redis:6379";
                return s;
            }
        }

        public static bool AllowAdmin => Convert.ToBoolean(DotEnv.Get(EnVars.REDIS_ALLOW_ADMIN) ?? "true");
    }


    public static class EnVars
    {
        public const string REDIS_PWD = "REDIS_PWD";
        public const string REDIS_USER = "REDIS_USER";
        public const string REDIS_CONFIG = "REDIS_CONFIG";
        public const string REDIS_USE_SSL = "REDIS_USE_SSL";
        public const string REDIS_ALLOW_ADMIN = "REDIS_ALLOW_ADMIN";
    }
}