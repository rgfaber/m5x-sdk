using System;
using System.Collections.Generic;
using M5x.Config;
using StackExchange.Redis;

namespace M5x.Redis
{
    public static class Config
    {
        public static readonly string Password = DotEnv.Get(EnVars.REDIS_PWD);
        public static readonly string User = DotEnv.Get(EnVars.REDIS_USER);
        public static readonly bool UseSsl = Convert.ToBoolean(DotEnv.Get(EnVars.REDIS_USE_SSL));
        public static string ConfigString = DotEnv.Get(EnVars.REDIS_CONFIG) ?? "";
        public static bool AllowAdmin = Convert.ToBoolean(DotEnv.Get(EnVars.REDIS_ALLOW_ADMIN));
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