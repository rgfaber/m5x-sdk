using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Kubernetes.IntegrationTests;

public class KubernetesTestsBase : IoCTestsBase
{
    public KubernetesTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected k8s.Kubernetes Client { get; set; }

    protected IKubernetesFactory Fact { get; set; }

    protected override void Initialize()
    {
        Fact = Container.GetService<IKubernetesFactory>();
        Client = Fact.Build();
    }

    protected override void SetTestEnvironment()
    {
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services.AddKubernetes();
    }


    [Fact]
    public void Should_ContainFactory()
    {
        Assert.NotNull(Fact);
    }

    [Fact]
    public void Should_ContainClient()
    {
        Assert.NotNull(Client);
    }
}