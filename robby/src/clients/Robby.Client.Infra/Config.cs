using M5x.Config;

namespace Robby.Client.Infra
{
    public static class Config
    {
        public static class QueryOptions
        {
            public static string Url = DotEnv.Get(EnVars.ROBBY_QRY_URL);
        }

        public static class HopeOptions
        {
            public static string Url = DotEnv.Get(EnVars.ROBBY_HOPE_URL);
        }
    }
}