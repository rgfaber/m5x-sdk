using System.Collections.Generic;
using M5x.Camunda.Transfer;

namespace M5x.Camunda.Requests
{
    public class FetchAndLockRequest
    {
        public string WorkerId { get; set; }
        public int MaxTasks { get; set; }
        public bool UsePriority { get; set; }
        public List<FetchAndLockTopic> Topics { get; set; } = new();
        public long AsyncResponseTimeout { get; set; }
    }
}