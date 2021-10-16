using System.Threading.Tasks;
using Docker.DotNet.Models;
using M5x.Docker.Models;

namespace M5x.Docker.Interfaces
{
    public interface IDockerContainer
    {
        Task<ContainerListResponse> Start(ContainerInfo nfo);
        Task<bool> VerifyService(ContainerInfo info);
    }
}