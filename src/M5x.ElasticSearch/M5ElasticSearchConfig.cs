using System;

namespace M5x.ElasticSearch
{
    public static class M5ElasticSearchConfig
    {
        public static string Url = Environment.GetEnvironmentVariable(EnVars.M5_ES_URL) ?? "http://es-svc:9200";
        public static string TimeOutMin = Environment.GetEnvironmentVariable(EnVars.M5_ES_TIMEOUT_MINS) ?? "2";
    }
}