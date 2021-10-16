using System;
using M5x.Kubernetes;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

namespace M5x.Stan
{
    public static class Inject
    {
        public static IServiceCollection AddJetStreamInfra(this IServiceCollection services)
        {
            services?
                //    .AddKubernetes();
                // var container = services.BuildServiceProvider(); 
                // var k8sFact = container.GetService<IKubernetesFactory>();
                // if (!k8sFact.InCluster) 
                //     services.AddStanInfraFromK8S();
                // else
                .AddNatsClient(options =>
                {
                    options.User = Config.User;
                    options.Password = Config.Password;
                    options.Servers = new[]
                    {
                        Config.Uri
                    };
                });
            return services;
        }


        public static IServiceCollection AddStan(this IServiceCollection services,
            Action<Options> options = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services?
                .AddKubernetes()
                .AddNatsClient(options, lifetime);
        }
    }
}