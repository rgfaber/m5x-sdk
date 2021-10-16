using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using M5x.Docker.Interfaces;
using M5x.Docker.Models;
using Serilog;

namespace M5x.Docker
{
    public abstract class DockerContainerBase : IDockerContainer
    {
        protected readonly IDockerEnvironment Denv;

        protected DockerContainerBase(IDockerEnvironment denv)
        {
            Denv = denv;
        }

        public async Task<bool> VerifyService(ContainerInfo info)
        {
            if (!info.ForceVerify) return true;
            var res = true;
            Thread.Sleep(3000);
            foreach (var url in info.VerifyUrls)
            {
                var b = await Denv.VerifyService(url);
                res = res && b;
            }

            return res;
        }

        public async Task<ContainerListResponse> Start(ContainerInfo nfo)
        {
            try
            {
                var startInfo = await Denv.StartContainer(nfo);
                if (startInfo != null)
                {
                    startInfo.WriteStartResult();
                    return startInfo;
                }

                throw new Exception($"Container '{nfo.ContainerName}' could not be started");
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
    }
}