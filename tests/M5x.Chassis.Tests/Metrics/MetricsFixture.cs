using System;
using M5x.Chassis.Mh;

namespace M5x.Chassis.Tests.Metrics
{
    public class MetricsFixture : IDisposable
    {
        public MetricsFixture()
        {
            M = new Mh.Metrics();
            H = new HealthChecks();
        }

        public Mh.Metrics M { get; }
        public HealthChecks H { get; }

        public void Dispose()
        {
            M.Dispose();
            H.Dispose();
        }
    }
}