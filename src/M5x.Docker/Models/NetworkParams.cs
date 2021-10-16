using System.Collections.Generic;

namespace M5x.Docker.Models
{
    public class NetworkParams
    {
        public NetworkParams()
        {
            IPAM = new IPAMInfo();
            Labels = new Dictionary<string, string>();
            Options = new Dictionary<string, string>();
        }

        public bool Attachable { get; set; }
        public bool CheckDuplicate { get; set; }
        public string Driver { get; set; }
        public bool EnableIPV6 { get; set; }
        public bool IsInternal { get; set; }
        public IPAMInfo IPAM { get; }
        public IDictionary<string, string> Labels { get; }
        public IDictionary<string, string> Options { get; }

        public string Name { get; set; }
    }
}