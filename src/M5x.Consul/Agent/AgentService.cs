namespace M5x.Consul.Agent
{
    /// <summary>
    ///     AgentService represents a service known to the agent
    /// </summary>
    public class AgentService
    {
        public string ID { get; set; }
        public string Service { get; set; }
        public string[] Tags { get; set; }
        public int Port { get; set; }
        public string Address { get; set; }
        public bool EnableTagOverride { get; set; }
    }
}