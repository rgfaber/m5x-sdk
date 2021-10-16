using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace M5x.ElasticSearch
{
    public static class Inject
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionSettingsValues>(new ConnectionSettings(new Uri(M5ElasticSearchConfig.Url)))
                .AddTransient<IElasticClient, ElasticClient>();
        }
    }
}