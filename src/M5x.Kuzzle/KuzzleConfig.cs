using System;

namespace M5x.Kuzzle
{
    public static class KuzzleConfig
    {
        public static string Url = Environment
            .GetEnvironmentVariable(EnVars.KUZZLE_URL) ?? "ws://kuzzle.local";
    }
}