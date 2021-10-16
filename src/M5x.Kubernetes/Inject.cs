using k8s;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Kubernetes
{
    public static class Inject
    {
        public static IServiceCollection AddKubernetes(this IServiceCollection services, string filename = null)
        {
            return services
                .AddSingleton(cfg => KubernetesClientConfiguration.IsInCluster()
                    ? KubernetesClientConfiguration.InClusterConfig()
                    : KubernetesClientConfiguration.BuildConfigFromConfigFile(filename))
                .AddSingleton<IKubernetesFactory, KubernetesFactory>();
        }
    }
}