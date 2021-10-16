using System.Collections.Generic;

namespace M5x.Camunda.Transfer
{
    public class FetchAndLockTopic
    {
        public string TopicName { get; set; }
        public long LockDuration { get; set; }
        public IEnumerable<string> Variables { get; set; }
    }
}