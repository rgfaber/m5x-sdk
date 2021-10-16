using System.Collections.Generic;
using Newtonsoft.Json;

namespace M5x.Consul.PreparedQuery
{
    /// <summary>
    ///     ServiceQuery is used to query for a set of healthy nodes offering a specific service.
    /// </summary>
    public class ServiceQuery
    {
        /// <summary>
        ///     Service is the service to query.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Service { get; set; }

        /// <summary>
        ///     Near allows baking in the name of a node to automatically distance-
        ///     sort from. The magic "_agent" value is supported, which sorts near
        ///     the agent which initiated the request by default.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Near { get; set; }

        /// <summary>
        ///     Failover controls what we do if there are no healthy nodes in the local datacenter.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public QueryDatacenterOptions Failover { get; set; }

        /// <summary>
        ///     If OnlyPassing is true then we will only include nodes with passing
        ///     health checks (critical AND warning checks will cause a node to be
        ///     discarded)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool OnlyPassing { get; set; }

        /// <summary>
        ///     Tags are a set of required and/or disallowed tags. If a tag is in
        ///     this list it must be present. If the tag is preceded with "!" then
        ///     it is disallowed.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tags { get; set; }
    }
}