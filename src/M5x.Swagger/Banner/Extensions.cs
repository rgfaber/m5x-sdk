namespace M5x.Swagger.Banner
{
    internal static class Extensions
    {
        public static string ToBanner(this IApiSettings api)
        {
            return "==========================\n" +
                   $"{api.Id}\n" +
                   $"Service  : {api.DisplayName}\n" +
                   $"Ver   : {api.Version}\n" +
                   $"Tenant: {api.TenantKey}\n" +
                   "===========================\n";
        }
    }
}