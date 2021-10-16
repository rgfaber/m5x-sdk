using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using M5x.Docker.Interfaces;
using M5x.Docker.Models;
using Serilog;

namespace M5x.Docker
{
    public class DockerEnvironment : IDisposable, IDockerEnvironment
    {
        private IDockerClient _client;

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task<bool> VerifyService(string url = null)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            using (var httpClient = new HttpClient())
            {
                while (true)
                    try
                    {
                        Log.Information($"Verify {url} => ");
                        Thread.Sleep(2000);
                        using var response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                            Log.Information("Ok");
                        return response.IsSuccessStatusCode;
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(e, e.Message);
                        return false;
                    }
                    finally
                    {
                        Log.CloseAndFlush();
                    }
            }
        }

        public async Task<ContainerListResponse> StartContainer(ContainerInfo info)
        {
            try
            {
                using var conf = new DockerClientConfiguration(DockerUtils.DaemonUri);
                using (_client = conf.CreateClient())
                {
                    var container = await FindContainer(info.ContainerName);
                    if (container != null)
                    {
                        if (info.ForceNewContainer)
                        {
                            await ClearContainer(info);
                            container = await CreateContainer(info);
                        }
                    }
                    else
                    {
                        container = await CreateContainer(info);
                    }

                    if (container == null) return null;
                    info.ContainerId = container.ID;
                    if (container.State == "running") return container;
                    var started = await _client
                        .Containers
                        .StartContainerAsync(container.ID, new ContainerStartParameters());
                    do
                    {
                        var cs = await WaitUntilRunning(info);
                        started = cs.Running;
                    } while (!started);

                    if (!started) throw new Exception("Cannot start the docker container");
                    if (info.VerifyUrls == null) return container;
                    if (!info.ForceVerify) return container;
                    Thread.Sleep(5000);
                    foreach (var url in info.VerifyUrls)
                    {
                        if (await VerifyService(url)) continue;
                        Log.Debug($"Unable to verify service {info.ContainerName} @ {url} ");
                    }

                    return container;
                }

                ;
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<Stream> BuildImage(Stream dockerFile, string tag)
        {
            using (var conf = new DockerClientConfiguration(DockerUtils.DaemonUri)) // localhost
            {
                using (_client = conf.CreateClient())
                {
                    return await _client
                        .Images
                        .BuildImageFromDockerfileAsync(dockerFile, new ImageBuildParameters
                        {
                            Tags = new List<string> { tag }
                        });
                }
            }
        }

        public async Task<ContainerInspectResponse> InspectContainer(ContainerInfo config)
        {
            using (var conf = new DockerClientConfiguration(DockerUtils.DaemonUri)) // localhost
            {
                using (_client = conf.CreateClient())
                {
                    var container = await FindContainer(config.ContainerName);
                    if (container == null) return null;
                    var resp = await _client
                        .Containers
                        .InspectContainerAsync(container.ID);
                    return resp;
                }
            }
        }

        public async Task<bool> StopContainer(string containerName, uint? waitBeforeKillSeconds = 10)
        {
            using (var conf = new DockerClientConfiguration(DockerUtils.DaemonUri)) // localhost
            {
                using (_client = conf.CreateClient())
                {
                    var c = await FindContainer(containerName);
                    if (c == null) return true;
                    Log.Debug($"Stopping Container {containerName}");
                    var isStopped = await _client
                        .Containers
                        .StopContainerAsync(c.ID, new ContainerStopParameters
                        {
                            WaitBeforeKillSeconds = waitBeforeKillSeconds
                        });
                    if (isStopped)
                        Log.Debug($"Container {containerName} has stopped.");
                    return isStopped;
                }
            }
        }

        public async Task<Stream> GetLogs(string containerName)
        {
            using var conf = new DockerClientConfiguration(DockerUtils.DaemonUri);
            using (_client = conf.CreateClient())
            {
                var c = await FindContainer(containerName);
                var res = await _client
                    .Containers
                    .GetContainerLogsAsync(c.ID, new ContainerLogsParameters());
                return res;
            }
        }

        public async Task<IList<IDictionary<string, string>>> DeleteImage(string imageName)
        {
            using var conf = new DockerClientConfiguration(DockerUtils.DaemonUri);
            using (_client = conf.CreateClient())
            {
                Log.Debug($"Deleting Image  {imageName}");
                var res = await _client.Images.DeleteImageAsync(imageName, new ImageDeleteParameters
                {
                    Force = true
                });
                if (res == null) return null;
                Log.Debug("---");
                foreach (var re in res)
                foreach (var it in re)
                    Log.Debug($"{it.Key}:{it.Value}");
                Log.Debug("---");
                return res;
            }
        }

        public async Task KillContainer(string containerName, string signal = "SIGTERM")
        {
            using var conf = new DockerClientConfiguration(DockerUtils.DaemonUri);
            using (_client = conf.CreateClient())
            {
                var c = await FindContainer(containerName);
                if (c == null) return;
                await _client
                    .Containers
                    .KillContainerAsync(c.ID, new ContainerKillParameters
                    {
                        Signal = signal
                    });
            }
        }

        public async Task<NetworkCreateInfo> CreateNetwork(NetworkParams network)
        {
            var createParams = new NetworksCreateParameters
            {
                Attachable = network.Attachable,
                CheckDuplicate = network.CheckDuplicate,
                Driver = network.Driver,
                EnableIPv6 = network.EnableIPV6,
                Internal = network.IsInternal,
                Labels = network.Labels,
                Options = network.Options,
                Name = network.Name,
                IPAM = network.IPAM?
                    .ToIPAM()
            };
            try
            {
                using (var conf = new DockerClientConfiguration(DockerUtils.DaemonUri)) // localhost
                {
                    using (_client = conf.CreateClient())
                    {
                        var res = await _client
                            .Networks
                            .CreateNetworkAsync(createParams);
                        return res?
                            .ToNetworkCreateInfo();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private async Task<ContainerState> WaitUntilRunning(ContainerInfo info)
        {
            ContainerInspectResponse resp = null;
            var container = await FindContainer(info.ContainerName);
            if (container != null)
            {
                var restartCount = info.MaxRestarts;
                var j = 0;
                do
                {
                    resp = await _client
                        .Containers
                        .InspectContainerAsync(container.ID);
                    if (resp.State.Dead)
                        throw new Exception($"Container: {container.ID} is dead!");
                    if (resp.State.Restarting)
                        restartCount--;
                    if (restartCount == 0 || resp.State.ExitCode == 1)
                    {
                        var logs = await _client
                            .Containers
                            .GetContainerLogsAsync(container.ID,
                                new ContainerLogsParameters { ShowStderr = true, ShowStdout = true });
                        throw new Exception($"Container is restarting/exited, gave up after {restartCount}  tries.\n" +
                                            "----------------------------------------------------\n" +
                                            "Log:\n" +
                                            ">>>>>>\n" +
                                            $"{logs.AsString()}\n" +
                                            "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    }

                    if (resp.State.Paused) continue;
                    j++;
                    Thread.Sleep(100 * j);
                } while (!resp.State.Running || j < 20);
            }

            ;
            return resp.State;
        }

        private async Task ClearContainer(ContainerInfo info)
        {
            var container = await FindContainer(info.ContainerName);
            if (container == null) return;
            Log.Debug($"Clearing container : {info.ContainerName}");
            await _client.Containers.RemoveContainerAsync(container.ID,
                new ContainerRemoveParameters
                {
                    Force = true
                });
            do
            {
                container = await FindContainer(info.ContainerName);
                if (container != null)
                    await DeleteImage(container.Image);
            } while (container != null);

            if (info.ForceNewImage) await DeleteImage(info.ImageName);
        }

        private async Task<ContainerListResponse> FindContainer(string containerName)
        {
            var containers = await _client
                .Containers
                .ListContainersAsync(new ContainersListParameters { All = true });
            try
            {
                if (containers == null || !containers.Any()) return null;
                var res = containers
                    .FirstOrDefault(c => c.Names.Contains("/" + containerName));
                return res;
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private async Task<ContainerListResponse> CreateContainer(ContainerInfo info)
        {
            Log.Debug($"Creating Container {info.ContainerName} ");
            if (!info.ImageName.StartsWith("local/")) await DownloadImage(info);
            // Create the container
            var config = new global::Docker.DotNet.Models.Config
            {
                Hostname = info.HostName
            };
            // Configure the ports to expose
            var hostConfig = new HostConfig
            {
                PortBindings = info.Ports.ToPortBindings()
            };
            // Create the container
            var createResponse = await _client
                .Containers
                .CreateContainerAsync(new CreateContainerParameters(config)
                {
                    Image = string.IsNullOrWhiteSpace(info.ImageTag)
                        ? info.ImageName
                        : info.ImageName + ":" + info.ImageTag,
                    Name = info.ContainerName,
                    Tty = false,
                    HostConfig = hostConfig,
                    Labels = info.Labels,
                    Env = info.Env,
                    Cmd = info.Cmd,
                    User = info.User,
                    Entrypoint = info.Entrypoint,
                    ExposedPorts = info.ExposedPorts.ToEmptyStructDict(),
                    Volumes = info.Volumes.ToEmptyStructDict()
                });
            // Get the container object
            var containers = await _client
                .Containers
                .ListContainersAsync(new ContainersListParameters { All = true });
            return containers.First(c => c.ID == createResponse.ID);
        }

        private async Task DownloadImage(ContainerInfo info)
        {
            if (info.ImageName.StartsWith("local/")) return;
            Log.Debug($"Downloading image {info.ImageName}:{info.ImageTag}");
            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = info.ImageName,
                    Tag = info.ImageTag
                },
                new AuthConfig(),
                new Progress<JSONMessage>());
        }
    }
}