using System.Collections.Generic;
using Newtonsoft.Json;

namespace M5x.Store.Models
{
    public class StoreInfo
    {
        public string CouchDb { get; set; }
        public string Uuid { get; set; }

        [JsonProperty("git_sha")] public string GitSha { get; set; }

        public string Version { get; set; }
        public Vendor Vendor { get; set; }
        public IEnumerable<string> Features { get; set; }
    }
}