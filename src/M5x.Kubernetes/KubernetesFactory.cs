using k8s;

namespace M5x.Kubernetes;

public interface IKubernetesFactory
{
    bool InCluster { get; }

    k8s.Kubernetes Build();
}

public class KubernetesFactory : IKubernetesFactory
{
    private readonly KubernetesClientConfiguration config;

    public KubernetesFactory(KubernetesClientConfiguration config)
    {
        this.config = config;
    }

    public k8s.Kubernetes Build()
    {
        return new k8s.Kubernetes(config);
    }

    public bool InCluster => KubernetesClientConfiguration.IsInCluster();
}