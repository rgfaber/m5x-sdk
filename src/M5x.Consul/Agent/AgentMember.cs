using System.Collections.Generic;

namespace M5x.Consul.Agent
{
    /// <summary>
    ///     AgentMember represents a cluster member known to the agent
    /// </summary>
    public class AgentMember
    {
        public AgentMember()
        {
            Tags = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public string Addr { get; set; }
        public ushort Port { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public int Status { get; set; }
        public byte ProtocolMin { get; set; }
        public byte ProtocolMax { get; set; }
        public byte ProtocolCur { get; set; }
        public byte DelegateMin { get; set; }
        public byte DelegateMax { get; set; }
        public byte DelegateCur { get; set; }
    }
}