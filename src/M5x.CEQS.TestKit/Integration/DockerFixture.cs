using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace M5x.CEQS.TestKit.Integration
{
    public class DockerFixture : IDisposable
    {
        private DockerClient _client;

        public void Dispose()
        {
            _client?.Dispose();
        }

        protected async Task StartContainer(string containerName, string imageName, string imageTag, string externalPort,
            string internalPort)
        {
            using (var conf = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"))) // localhost
            {
                _client = conf.CreateClient();
            }

            var containers = await _client
                .Containers
                .ListContainersAsync(new ContainersListParameters {All = true});
            var container = containers
                .FirstOrDefault(c => c.Names.Contains("/" + containerName));
            if (container == null)
            {
                // Download image
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters {FromImage = imageName, Tag = imageTag}, new AuthConfig(),
                    new Progress<JSONMessage>());
                // Create the container
                var config = new global::Docker.DotNet.Models.Config
                {
                    Hostname = "localhost"
                };
                // Configure the ports to expose
                var hostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            internalPort,
                            new List<PortBinding>
                            {
                                new() {HostIP = "127.0.0.1", HostPort = externalPort}
                            }
                        }
                    }
                };
                // Create the container
                var response = await _client
                    .Containers
                    .CreateContainerAsync(
                        new CreateContainerParameters(config)
                        {
                            Image = imageName + ":" + imageTag,
                            Name = containerName,
                            Tty = false,
                            HostConfig = hostConfig
                        });
                // Get the container object
                containers = await _client
                    .Containers
                    .ListContainersAsync(new ContainersListParameters {All = true});
                container = containers.First(c => c.ID == response.ID);
            }

            if (container.State != "running")
            {
                var started = await _client
                    .Containers
                    .StartContainerAsync(container.ID, new ContainerStartParameters());
                if (!started) throw new Exception("Cannot start the docker container");
            }
        }
    }
}