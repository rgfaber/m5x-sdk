using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyNatsClient;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Nats.Tests;

public abstract class NatsTestsBase : IoCTestsBase
{
    public NatsTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected INatsClient Client { get; set; }

    protected override void Initialize()
    {
        Client = Container.GetService<INatsClient>();
        Assert.NotNull(Client);
    }


    protected override void InjectDependencies(IServiceCollection services)
    {
        services.AddNats();
    }
}