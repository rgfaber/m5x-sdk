using Newtonsoft.Json;

namespace M5x.Consul.ACL;

/// <summary>
///     ACLEntry is used to represent an ACL entry
/// </summary>
public class ACLEntry
{
    public ACLEntry()
        : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public ACLEntry(string name, string rules)
        : this(string.Empty, name, rules)
    {
    }

    public ACLEntry(string id, string name, string rules)
    {
        Type = ACLType.Client;
        Id = id;
        Name = name;
        Rules = rules;
    }

    public ulong CreateIndex { get; set; }
    public ulong ModifyIndex { get; set; }

    public string Id { get; set; }
    public string Name { get; set; }

    [JsonConverter(typeof(AclTypeConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ACLType Type { get; set; }

    public string Rules { get; set; }

    public bool ShouldSerializeCreateIndex()
    {
        return false;
    }

    public bool ShouldSerializeModifyIndex()
    {
        return false;
    }
}