using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using M5x.Docker.Models;

namespace M5x.Docker.Interfaces
{
    public interface IDockerEnvironment
    {
        Task<bool> VerifyService(string url);
        Task<ContainerListResponse> StartContainer(ContainerInfo info);
        Task<bool> StopContainer(string containerName, uint? waitBeforeKillSeconds = 10);
        Task<Stream> GetLogs(string containerName);
        Task KillContainer(string containerName, string signal = "SIGTERM");
        Task<NetworkCreateInfo> CreateNetwork(NetworkParams info);
        Task<Stream> BuildImage(Stream dockerFile, string tag);
        Task<ContainerInspectResponse> InspectContainer(ContainerInfo config);
        Task<IList<IDictionary<string, string>>> DeleteImage(string imageName);
    }
}