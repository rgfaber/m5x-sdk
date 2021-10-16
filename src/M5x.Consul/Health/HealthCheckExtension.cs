using System.Collections.Generic;

namespace M5x.Consul.Health
{
    public static class HealthCheckExtension
    {
        public static HealthStatus AggregatedStatus(this IEnumerable<HealthCheck> checks)
        {
            if (checks == null) return HealthStatus.Passing;

            bool warning = false, critical = false, maintenance = false;
            foreach (var check in checks)
                if (!string.IsNullOrEmpty(check.CheckId) &&
                    (check.CheckId == HealthStatus.NodeMaintenance ||
                     check.CheckId.StartsWith(HealthStatus.ServiceMaintenancePrefix)))
                {
                    maintenance = true;
                    break;
                }
                else if (check.Status == HealthStatus.Critical)
                {
                    critical = true;
                }
                else if (check.Status == HealthStatus.Warning)
                {
                    warning = true;
                }

            if (maintenance)
                return HealthStatus.Maintenance;
            if (critical)
                return HealthStatus.Critical;
            if (warning)
                return HealthStatus.Warning;
            return HealthStatus.Passing;
        }
    }
}