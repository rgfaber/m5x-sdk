using System.Threading.Tasks;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Kubernetes.IntegrationTests;

public class KubernetesTests : KubernetesTestsBase
{
    public KubernetesTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public async Task Should_RetrieveSecrets()
    {
        var rsp = await Client.ListSecretForAllNamespacesWithHttpMessagesAsync();
        var lst = rsp.Body;
        foreach (var v1Secret in lst.Items) Output.WriteLine(v1Secret.Metadata.Name);
    }
}