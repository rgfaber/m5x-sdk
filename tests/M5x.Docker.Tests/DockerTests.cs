using System.Collections.Generic;
using M5x.Docker.Interfaces;
using M5x.Docker.Models;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Docker.Tests
{
    public class DockerTests : IoCTestsBase
    {
        public DockerTests(ITestOutputHelper output, IoCTestContainer container)
            : base(output, container)
        {
        }


        protected override void Initialize()
        {
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddDockerEnvironment();
        }


        [Fact]
        public async void Try_CreateNetwork()
        {
            var docker = Container.GetService<IDockerEnvironment>();
            Assert.NotNull(docker);
            var res = await docker.CreateNetwork(new NetworkParams { Driver = "bridge", Name = "test-net" });
        }

        [Fact]
        public async void Try_StartContainer()
        {
            var docker = Container.GetService<IDockerEnvironment>();
            Assert.NotNull(docker);
            var nfo = new ContainerInfo
            {
                HostName = "localhost",
                ContainerName = "nginx-test",
                ImageName = "nginx",
                ImageTag = "latest",
                Ports = new Dictionary<string, string>
                {
                    { "80", "8080" }
                },
                ExposedPorts = new List<string>
                {
                    "80"
                },
                VerifyUrls = new List<string> { "http://localhost:8080" },
                ForceVerify = true,
                ForceNewImage = true,
                ForceNewContainer = true
            };

            await docker.StartContainer(nfo);
            await docker.VerifyService("http://localhost:8080");
            //   await docker.StopContainer(nfo.ContainerName);
        }
    }
}