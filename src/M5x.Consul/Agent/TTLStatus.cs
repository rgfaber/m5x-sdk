using System;

namespace M5x.Consul.Agent
{
    /// <summary>
    ///     The status of a TTL check
    /// </summary>
    public class TtlStatus : IEquatable<TtlStatus>
    {
        public string Status { get; private set; }
        internal string LegacyStatus { get; private set; }

        public static TtlStatus Pass { get; } = new() { Status = "passing", LegacyStatus = "pass" };

        public static TtlStatus Warn { get; } = new() { Status = "warning", LegacyStatus = "warn" };

        public static TtlStatus Critical { get; } = new() { Status = "critical", LegacyStatus = "fail" };

        [Obsolete("Use TTLStatus.Critical instead. This status will be an error in 0.7.0+", true)]
        public static TtlStatus Fail => Critical;

        public bool Equals(TtlStatus other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        public override bool Equals(object other)
        {
            // other could be a reference type, the is operator will return false if null
            return other is TtlStatus && Equals(other as TtlStatus);
        }

        public override int GetHashCode()
        {
            return Status.GetHashCode();
        }
    }
}