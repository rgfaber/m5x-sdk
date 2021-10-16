using Microsoft.Extensions.DependencyInjection;

namespace M5x.Yaml
{
    public static class Inject
    {
        public static IServiceCollection AddYaml(this IServiceCollection services)
        {
            return services?
                .AddTransient<IYamlSerializer, YamlSerializer>()
                .AddTransient<IYamlDeserializer, YamlDeserializer>();
        }
    }
}