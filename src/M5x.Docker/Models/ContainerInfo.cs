using System.Collections.Generic;

namespace M5x.Docker.Models
{
    public class ContainerInfo
    {
        public ContainerInfo()
        {
            Ports = new Dictionary<string, string>();
            HostName = "localhost";
            Labels = new Dictionary<string, string>();
            Env = new List<string>();
            Cmd = new List<string>();
            Entrypoint = new List<string>();
            Volumes = new List<string>();
            ForceNewImage = false;
            ForceNewContainer = false;
            ForceVerify = false;
            MaxRestarts = 5;
        }

        public bool ForceVerify { get; set; }

        public string ContainerName { get; set; }
        public string ImageName { get; set; }
        public string ImageTag { get; set; }
        public IDictionary<string, string> Ports { get; set; }
        public string HostName { get; set; }
        public IDictionary<string, string> Labels { get; set; }
        public IList<string> Env { get; set; }
        public IList<string> Cmd { get; set; }
        public string User { get; set; }
        public IList<string> Entrypoint { get; set; }
        public IList<string> ExposedPorts { get; set; }
        public string ContainerId { get; set; }
        public IList<string> Volumes { get; set; }
        public IList<string> VerifyUrls { get; set; }
        public bool ForceNewContainer { get; set; }
        public bool ForceNewImage { get; set; }
        public int MaxRestarts { get; set; }
    }
}