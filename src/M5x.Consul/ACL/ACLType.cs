using System;

namespace M5x.Consul.ACL
{
    /// <summary>
    ///     The type of ACL token, which sets the permissions ceiling
    /// </summary>
    public class ACLType : IEquatable<ACLType>
    {
        public string Type { get; private set; }

        /// <summary>
        ///     Token type which cannot modify ACL rules
        /// </summary>
        public static ACLType Client => new() { Type = "client" };

        /// <summary>
        ///     Token type which is allowed to perform all actions
        /// </summary>
        public static ACLType Management => new() { Type = "management" };

        public bool Equals(ACLType other)
        {
            if (other == null) return false;
            return Type.Equals(other.Type);
        }

        public override bool Equals(object other)
        {
            var a = other as ACLType;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}