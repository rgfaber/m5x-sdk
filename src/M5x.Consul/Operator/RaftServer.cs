namespace M5x.Consul.Operator;

/// <summary>
///     RaftServer has information about a server in the Raft configuration.
/// </summary>
public class RaftServer
{
    /// <summary>
    ///     ID is the unique ID for the server. These are currently the same
    ///     as the address, but they will be changed to a real GUID in a future
    ///     release of Consul.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     Node is the node name of the server, as known by Consul, or this
    ///     will be set to "(unknown)" otherwise.
    /// </summary>
    public string Node { get; set; }

    /// <summary>
    ///     CONSUL_HTTP_ADDRESS is the IP:port of the server, used for Raft communications.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///     Leader is true if this server is the current cluster leader.
    /// </summary>
    public bool Leader { get; set; }

    /// <summary>
    ///     Voter is true if this server has a vote in the cluster. This might
    ///     be false if the server is staging and still coming online, or if
    ///     it's a non-voting server, which will be added in a future release of
    ///     Consul
    /// </summary>
    public bool Voter { get; set; }
}