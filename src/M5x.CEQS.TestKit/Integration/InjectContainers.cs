using M5x.CEQS.TestKit.Integration.Containers;
using M5x.Docker;
using M5x.Docker.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.CEQS.TestKit.Integration
{
    public static class InjectContainers
    {
        
        public static IServiceCollection AddConsulContainer(this IServiceCollection services)
        {
            return services?
                .AddDockerEnvironment()
                .AddSingleton<IConsulContainer, ConsulContainer>();
        }


        public static IServiceCollection AddCouchContainer(this IServiceCollection services)
        {
            return services?
                .AddDockerEnvironment()
                .AddSingleton<ICouchContainer, CouchContainer>();
        }


        public static IServiceCollection AddNatsContainer(this IServiceCollection services)
        {
            return services?
                .AddDockerEnvironment()
                .AddSingleton<INatsContainer, NatsContainer>();
        }
        
    }
}