using System.Collections.Generic;

namespace M5x.Docker.Models;

public class IPAMInfo
{
    public IPAMInfo()
    {
        Config = new List<IPAMConfigInfo>();
        Options = new Dictionary<string, string>();
        Driver = "bridge";
    }

    public string Driver { get; set; }
    public IDictionary<string, string> Options { get; set; }
    public IList<IPAMConfigInfo> Config { get; }
}