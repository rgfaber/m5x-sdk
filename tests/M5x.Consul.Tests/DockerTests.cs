using System.Threading.Tasks;
using M5x.CEQS.TestKit.Integration.Containers;
using M5x.Consul.Agent;
using M5x.Consul.Interfaces;
using M5x.Docker.Interfaces;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class DockerTests : ConsulTestsBase
    {
        public DockerTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public async Task Try_RegisterWithConsul()
        {
            var consul = Container.GetRequiredService<IConsulClient>();
            Assert.NotNull(consul);
            var srv = Container.GetRequiredService<IConsulContainer>();
            var reg = await consul.Agent.ServiceRegister(new AgentServiceRegistration
            {
                Name = "Try_RegisterWithConsul",
                Address = "localhost",
                Id = "M5x.Consul.Tests.Try_RegisterWithConsul",
                Port = 8055
            });
            Assert.NotNull(reg);
            var res = consul.Agent.Services().Result;
            Assert.NotNull(res);
        }
    }
}