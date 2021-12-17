using System.Collections.Generic;
using Newtonsoft.Json;

namespace M5x.Consul.Catalog;

public class Node
{
    // Cannot be "Node" as in the Go API because in C#, properties cannot
    // have the same name as their enclosing class.
    [JsonProperty(PropertyName = "Node")] public string Name { get; set; }

    public string Address { get; set; }
    public Dictionary<string, string> TaggedAddresses { get; set; }
}