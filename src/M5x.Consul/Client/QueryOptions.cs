using System;

namespace M5x.Consul.Client
{
    /// <summary>
    ///     QueryOptions are used to parameterize a query
    /// </summary>
    public class QueryOptions
    {
        public static readonly QueryOptions Default = new()
        {
            Consistency = ConsistencyMode.Default,
            Datacenter = string.Empty,
            Token = string.Empty,
            WaitIndex = 0
        };

        /// <summary>
        ///     Providing a datacenter overwrites the DC provided by the Config
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        ///     The consistency level required for the operation
        /// </summary>
        public ConsistencyMode Consistency { get; set; }

        /// <summary>
        ///     WaitIndex is used to enable a blocking query. Waits until the timeout or the next index is reached
        /// </summary>
        public ulong WaitIndex { get; set; }

        /// <summary>
        ///     WaitTime is used to bound the duration of a wait. Defaults to that of the Config, but can be overridden.
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        ///     Token is used to provide a per-request ACL token which overrides the agent's default token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Near is used to provide a node name that will sort the results
        ///     in ascending order based on the estimated round trip time from
        ///     that node. Setting this to "_agent" will use the agent's node
        ///     for the sort.
        /// </summary>
        public string Near { get; set; }
    }
}