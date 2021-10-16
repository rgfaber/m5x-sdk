using M5x.Chassis.Container;
using M5x.Chassis.Container.Interfaces;
using M5x.Chassis.Mh;

namespace M5x.Chassis.Service.Metrics
{
    public partial class Add
    {
        internal static void RegisterMetricsAndHealthChecks(this IContainer container)
        {
            container.Register("M", () => new Mh.Metrics(), Lifetime.Permanent);
            container.Register("H", () => new HealthChecks(), Lifetime.Permanent);
        }
    }
}