using System;
using System.Collections.Generic;
using System.Text;
using M5x.Chassis.Mh.Core;
using M5x.Chassis.Mh.Util;

namespace M5x.Chassis.Mh.Reporting
{
    public class HumanReadableReportFormatter : IReportFormatter
    {
        private readonly Func<IDictionary<MetricName, IMetric>> _producer;

        public HumanReadableReportFormatter(Metrics metrics) : this(() => metrics.AllSorted)
        {
        }

        public HumanReadableReportFormatter(HealthChecks healthChecks) : this(healthChecks.RunHealthChecks)
        {
        }

        public HumanReadableReportFormatter(Func<IDictionary<MetricName, IMetric>> producer)
        {
            _producer = producer;
        }

        public string GetSample()
        {
            var sb = new StringBuilder();
            var now = DateTime.UtcNow;
            var dateTime = string.Format("{0} {1}", now.ToString("d"), now.ToString("t"));
            sb.Append(dateTime);
            sb.Append(' ');
            for (var i = 0; i < 80 - dateTime.Length - 1; i++) sb.Append('=');
            sb.AppendLine();

            foreach (var entry in HelperUtils.SortMetrics(_producer()))
            {
                sb.Append(entry.Key);
                sb.AppendLine(":");

                foreach (var subEntry in entry.Value)
                {
                    sb.Append("  ");
                    sb.Append(subEntry.Key);
                    sb.AppendLine(":");

                    var metric = subEntry.Value;
                    var gauge = metric as GaugeMetric;
                    if (gauge != null)
                    {
                        WriteGauge(sb, gauge);
                    }
                    else
                    {
                        var counter = metric as CounterMetric;
                        if (counter != null)
                        {
                            WriteCounter(sb, counter);
                        }
                        else
                        {
                            var histogram = metric as HistogramMetric;
                            if (histogram != null)
                            {
                                WriteHistogram(sb, histogram);
                            }
                            else
                            {
                                var meter = metric as MeterMetric;
                                if (meter != null)
                                {
                                    WriteMetered(sb, meter);
                                }
                                else
                                {
                                    var timer = metric as TimerMetricBase;
                                    if (timer != null) WriteTimer(sb, timer);
                                }
                            }
                        }
                    }

                    sb.AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void WriteGauge(StringBuilder sb, GaugeMetric gauge)
        {
            sb.Append("    value = ");
            sb.AppendLine(gauge.ValueAsString);
        }

        private static void WriteCounter(StringBuilder sb, CounterMetric counter)
        {
            sb.Append("    count = ");
            sb.AppendLine(counter.Count.ToString());
        }

        private static void WriteMetered(StringBuilder sb, IMetered meter)
        {
            var unit = Abbreviate(meter.RateUnit);
            sb.AppendFormat("             count = {0}\n", meter.Count);
            sb.AppendFormat("         mean rate = {0} {1}/{2}\n", meter.MeanRate, meter.EventType, unit);
            sb.AppendFormat("     1-minute rate = {0} {1}/{2}\n", meter.OneMinuteRate, meter.EventType, unit);
            sb.AppendFormat("     5-minute rate = {0} {1}/{2}\n", meter.FiveMinuteRate, meter.EventType, unit);
            sb.AppendFormat("    15-minute rate = {0} {1}/{2}\n", meter.FifteenMinuteRate, meter.EventType, unit);
        }

        private static void WriteHistogram(StringBuilder sb, HistogramMetric histogram)
        {
            var percentiles = histogram.Percentiles(0.5, 0.75, 0.95, 0.98, 0.99, 0.999);

            sb.AppendFormat("               min = {0:F2}\n", histogram.Min);
            sb.AppendFormat("               max = {0:F2}\n", histogram.Max);
            sb.AppendFormat("              mean = {0:F2}\n", histogram.Mean);
            sb.AppendFormat("            stddev = {0:F2}\n", histogram.StdDev);
            sb.AppendFormat("            median = {0:F2}\n", percentiles[0]);
            sb.AppendFormat("              75%% <= {0:F2}\n", percentiles[1]);
            sb.AppendFormat("              95%% <= {0:F2}\n", percentiles[2]);
            sb.AppendFormat("              98%% <= {0:F2}\n", percentiles[3]);
            sb.AppendFormat("              99%% <= {0:F2}\n", percentiles[4]);
            sb.AppendFormat("            99.9%% <= {0:F2}\n", percentiles[5]);
        }

        private static void WriteTimer(StringBuilder sb, TimerMetricBase timer)
        {
            WriteMetered(sb, timer);

            var durationUnit = Abbreviate(timer.DurationUnit);

            var percentiles = timer.Percentiles(0.5, 0.75, 0.95, 0.98, 0.99, 0.999);

            sb.AppendFormat("               min = {0:F2}{1}\n", timer.Min, durationUnit);
            sb.AppendFormat("               max = {0:F2}{1}\n", timer.Max, durationUnit);
            sb.AppendFormat("              mean = {0:F2}{1}\n", timer.Mean, durationUnit);
            sb.AppendFormat("            stddev = {0:F2}{1}\n", timer.StdDev, durationUnit);
            sb.AppendFormat("            median = {0:F2}{1}\n", percentiles[0], durationUnit);
            sb.AppendFormat("              75%% <= {0:F2}{1}\n", percentiles[1], durationUnit);
            sb.AppendFormat("              95%% <= {0:F2}{1}\n", percentiles[2], durationUnit);
            sb.AppendFormat("              98%% <= {0:F2}{1}\n", percentiles[3], durationUnit);
            sb.AppendFormat("              99%% <= {0:F2}{1}\n", percentiles[4], durationUnit);
            sb.AppendFormat("            99.9%% <= {0:F2}{1}\n", percentiles[5], durationUnit);
        }

        private static string Abbreviate(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Nanoseconds:
                    return "ns";
                case TimeUnit.Microseconds:
                    return "us";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Minutes:
                    return "m";
                case TimeUnit.Hours:
                    return "h";
                case TimeUnit.Days:
                    return "d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit));
            }
        }
    }
}