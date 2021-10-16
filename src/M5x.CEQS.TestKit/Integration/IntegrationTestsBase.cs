using M5x.Docker;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Integration
{
    public abstract class IntegrationTestsBase : IoCTestsBase, IClassFixture<DockerFixture>
    {
        protected readonly DockerFixture Docker;

        protected IntegrationTestsBase(ITestOutputHelper output, IoCTestContainer container, DockerFixture docker)
            : base(output, container)
        {
            Docker = docker;
        }

    }
}