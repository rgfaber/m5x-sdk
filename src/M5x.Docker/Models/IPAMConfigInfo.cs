using System.Collections.Generic;

namespace M5x.Docker.Models;

public class IPAMConfigInfo
{
    public IPAMConfigInfo()
    {
        AuxAddress = new Dictionary<string, string>();
    }

    public string Subnet { get; set; }
    public string IPRange { get; set; }
    public string Gateway { get; set; }
    public IDictionary<string, string> AuxAddress { get; }
}