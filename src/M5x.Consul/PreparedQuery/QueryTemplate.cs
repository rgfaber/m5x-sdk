using Newtonsoft.Json;

namespace M5x.Consul.PreparedQuery
{
    /// <summary>
    ///     QueryTemplate carries the arguments for creating a templated query.
    /// </summary>
    public class QueryTemplate
    {
        public QueryTemplate()
        {
            Type = "name_prefix_match";
        }

        /// <summary>
        ///     Type specifies the type of the query template. Currently only
        ///     "name_prefix_match" is supported. This field is required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string Type { get; set; }

        /// <summary>
        ///     Regexp allows specifying a regex pattern to match against the name
        ///     of the query being executed.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regexp { get; set; }
    }
}