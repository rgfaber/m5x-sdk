using M5x.Couch.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Couch.Tests
{
    public static class Inject
    {
        public static IServiceCollection AddCouchDbTests(this IServiceCollection services)
        {
            return services?
                .AddSingleton<ICouchTests, CouchTests>();
        }
    }
}