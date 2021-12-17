using M5x.Kubernetes;
using NATS.Client;

namespace M5x.Stan;

internal class K8sStanClient : IK8sStanClient
{
    private readonly IKubernetesFactory _k8SFact;
    private readonly INatsClientConnectionFactory _natsFact;

    public K8sStanClient(INatsClientConnectionFactory natsFact, IKubernetesFactory k8sFact)
    {
        _natsFact = natsFact;
        _k8SFact = k8sFact;
    }
}

public interface IK8sStanClient
{
}