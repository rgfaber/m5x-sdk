using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Store
{
    public class ReplicationDoc : DocBase
    {
        [JsonProperty("user_ctx")] public UserContext Context { get; set; }

        public ReplicationPoint Source { get; set; }

        public ReplicationPoint Target { get; set; }
        //public string Source { get; set; }
        //public string Target { get; set; }

        public string Owner { get; set; }

        [JsonProperty("create_target")] public bool CreateTarget { get; set; }

        public bool Continuous { get; set; }
        public string Filter { get; set; }
    }

    public class ReplicationPoint
    {
        public JObject Headers { get; set; }
        public string Url { get; set; }
    }

    public class UserContext
    {
        public string Name { get; set; }
    }
}