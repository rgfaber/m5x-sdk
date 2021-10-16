using System;
using M5x.Config;

namespace M5x.Polly
{
    public static class Config
    {
        public static int MaxRetries = Convert.ToInt32(DotEnv.Get(EnVars.POLLY_RETRIES) ?? "100");
    }
}