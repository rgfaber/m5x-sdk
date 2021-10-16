using System;
using System.Collections.Generic;
using M5x.Chassis.Mh.Core;
using M5x.Chassis.Mh.Util;

namespace M5x.Chassis.Mh.Reporting
{
    public class MhReportFormatter : IReportFormatter
    {
        private readonly Func<IDictionary<MetricName, IMetric>> _producer;

        public MhReportFormatter(Metrics metrics) : this(() => metrics.AllSorted)
        {
        }

        public MhReportFormatter(HealthChecks healthChecks) : this(healthChecks.RunHealthChecks)
        {
        }

        public MhReportFormatter(Func<IDictionary<MetricName, IMetric>> producer)
        {
            _producer = producer;
        }

        public string GetSample()
        {
            return Serializer.Serialize(_producer());
        }
    }
}