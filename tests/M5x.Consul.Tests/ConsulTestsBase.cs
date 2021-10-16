using System.Threading.Tasks;
using Docker.DotNet.Models;
using M5x.CEQS.TestKit.Integration;
using M5x.CEQS.TestKit.Integration.Containers;
using M5x.Docker;
using M5x.Docker.Interfaces;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public abstract class ConsulTestsBase : IoCTestsBase
    {
        private IConsulContainer  cont;

        protected ConsulTestsBase(ITestOutputHelper output, IoCTestContainer container)
            : base(output, container)

        {
            // m_lock = AsyncHelpers.RunSync(() => SelectiveParallel.Parallel());
        }

        public new void Dispose()
        {
            // m_lock.Dispose();
        }


        //protected readonly AsyncReaderWriterLock.Releaser m_lock;


        protected override void SetTestEnvironment()
        {
            DockerConfig.SetTestEnvironment();
            ConsulConfig.SetTestEnvironment();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddConsulContainer()
                .AddConsul();
        }


        protected override void Initialize()
        {
            cont = Container.GetRequiredService<IConsulContainer>();
            Assert.NotNull(cont);
            ContainerListResponse res;
            do
            {
                res = Try_StartConsulContainer().Result;
            } while (res == null || res.State != "running");
        }


        private async Task<ContainerListResponse> Try_StartConsulContainer()
        {
            var res = await cont.Start();
            Assert.NotNull(res);
            Assert.True(res.State == "running");
            return res;
        }
    }
}