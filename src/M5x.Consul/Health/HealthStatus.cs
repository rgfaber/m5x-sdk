using System;

namespace M5x.Consul.Health
{
    /// <summary>
    ///     The status of a health check
    /// </summary>
    public class HealthStatus : IEquatable<HealthStatus>
    {
        public const string NodeMaintenance = "_node_maintenance";
        public const string ServiceMaintenancePrefix = "_service_maintenance:";

        public string Status { get; private set; }

        public static HealthStatus Passing { get; } = new() { Status = "passing" };

        public static HealthStatus Warning { get; } = new() { Status = "warning" };

        public static HealthStatus Critical { get; } = new() { Status = "critical" };

        public static HealthStatus Maintenance { get; } = new() { Status = "maintenance" };

        public static HealthStatus Any { get; } = new() { Status = "any" };

        public bool Equals(HealthStatus other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        public override bool Equals(object other)
        {
            // other could be a reference type, the is operator will return false if null
            return other is HealthStatus && Equals(other as HealthStatus);
        }

        public override int GetHashCode()
        {
            return Status.GetHashCode();
        }
    }
}