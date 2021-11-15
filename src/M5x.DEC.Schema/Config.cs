using System;
using M5x.Config;

namespace M5x.DEC.Schema
{
    public enum IDCreationPolicy
    {
        Strict = 1,
        Liberal = 2
    }


    public static class Config
    {
        public static IDCreationPolicy IdCreationPolicy =
            (IDCreationPolicy)Convert.ToInt32(DotEnv.Get(EnVars.DEC_ID_CREATION_POLICY) ?? "1");
    }

    public static class EnVars
    {
        public const string DEC_ID_CREATION_POLICY = "DEC_ID_CREATION_POLICY";
    }
}