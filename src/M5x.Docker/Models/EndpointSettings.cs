using System.Collections.Generic;

namespace M5x.Docker.Models;

public class EndpointSettings
{
    public EndpointIPAMConfig IPAMConfig { get; set; }
    public IList<string> Links { get; set; }
    public IList<string> Aliases { get; set; }
    public string NetworkID { get; set; }
    public string EndpointID { get; set; }
    public string Gateway { get; set; }
    public string IPAddress { get; set; }
    public long IPPrefixLen { get; set; }
    public string IPv6Gateway { get; set; }
    public string GlobalIPv6Address { get; set; }
    public long GlobalIPv6PrefixLen { get; set; }
    public string MacAddress { get; set; }
}