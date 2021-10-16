using Microsoft.Extensions.DependencyInjection;

namespace M5x.Yaml.Tests
{
    public static class Inject
    {
        public static IServiceCollection AddTestClassGenerator(this IServiceCollection services)
        {
            return services?
                .AddYaml()
                .AddSingleton<ITestClassGenerator, TestClassYamlGenerator>();
        }
    }
}