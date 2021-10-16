using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Integration
{
    public abstract class DockerTestsBase : IoCTestsBase, IClassFixture<DockerFixture>
    {
        private readonly DockerFixture _docker;

        protected DockerTestsBase(ITestOutputHelper output, IoCTestContainer container, DockerFixture docker)
            : base(output, container)
        {
            _docker = docker;
        }
    }
}