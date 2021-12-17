using M5x.Consul.Health;

namespace M5x.Consul.PreparedQuery;

/// <summary>
///     PreparedQueryExecuteResponse has the results of executing a query.
/// </summary>
public class PreparedQueryExecuteResponse
{
    /// <summary>
    ///     Service is the service that was queried.
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    ///     Nodes has the nodes that were output by the query.
    /// </summary>
    public ServiceEntry[] Nodes { get; set; }

    /// <summary>
    ///     DNS has the options for serving these results over DNS.
    /// </summary>
    public QueryDnsOptions Dns { get; set; }

    /// <summary>
    ///     Datacenter is the datacenter that these results came from.
    /// </summary>
    public string Datacenter { get; set; }

    /// <summary>
    ///     Failovers is a count of how many times we had to query a remote
    ///     datacenter.
    /// </summary>
    public int Failovers { get; set; }
}