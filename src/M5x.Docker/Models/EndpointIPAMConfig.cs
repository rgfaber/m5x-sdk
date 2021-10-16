using System.Collections.Generic;

namespace M5x.Docker.Models
{
    public class EndpointIPAMConfig
    {
        public string IPv4Address { get; set; }


        public string IPv6Address { get; set; }


        public IList<string> LinkLocalIPs { get; set; }
    }
}