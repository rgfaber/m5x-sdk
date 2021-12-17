﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using M5x.Chassis.Mh.Core;
using M5x.Chassis.Mh.Reporting;

namespace M5x.Chassis.Mh;

/// <summary>
///     A set of factory methods for creating centrally registered metric instances
/// </summary>
/// <see href="https://github.com/codahale/metrics" />
/// <seealso href="http://codahale.com/codeconf-2011-04-09-metrics-metrics-everywhere.pdf" />
public class Metrics : IDisposable
{
    private readonly ConcurrentDictionary<MetricName, IMetric> _metrics =
        new();

    /// <summary>
    ///     Returns a copy of all currently registered metrics in an immutable collection
    /// </summary>
    public IDictionary<MetricName, IMetric> All => new ReadOnlyDictionary<MetricName, IMetric>(_metrics);

    /// <summary>
    ///     Returns a copy of all currently registered metrics in an immutable collection, sorted by owner and name
    /// </summary>
    public IDictionary<MetricName, IMetric> AllSorted =>
        new ReadOnlyDictionary<MetricName, IMetric>(new SortedDictionary<MetricName, IMetric>(_metrics));

    public void Dispose()
    {
        foreach (var metric in _metrics)
            using (metric.Value as IDisposable)
            {
            }
    }

    /// <summary>
    ///     Creates a new gauge metric and registers it under the given type and name
    /// </summary>
    /// <typeparam name="T">The type the gauge measures</typeparam>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="evaluator">The gauge evaluation function</param>
    /// <returns></returns>
    public GaugeMetric<T> Gauge<T>(Type owner, string name, Func<T> evaluator)
    {
        return GetOrAdd(new MetricName(owner, name), new GaugeMetric<T>(evaluator));
    }

    /// <summary>
    ///     Creates a new gauge metric and registers it under the given type and name
    /// </summary>
    /// <typeparam name="T">The type the gauge measures</typeparam>
    /// <param name="context">The context for this metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="evaluator">The gauge evaluation function</param>
    /// <returns></returns>
    public GaugeMetric<T> Gauge<T>(string context, string name, Func<T> evaluator)
    {
        return GetOrAdd(new MetricName(context, name), new GaugeMetric<T>(evaluator));
    }

    /// <summary>
    ///     Creates a new counter metric and registers it under the given type and name
    /// </summary>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <returns></returns>
    public CounterMetric Counter(Type owner, string name)
    {
        return GetOrAdd(new MetricName(owner, name), new CounterMetric());
    }

    /// <summary>
    ///     Creates a new counter metric and registers it under the given type and name
    /// </summary>
    /// <param name="context">The context for this metric</param>
    /// <param name="name">The metric name</param>
    /// <returns></returns>
    public CounterMetric Counter(string context, string name)
    {
        return GetOrAdd(new MetricName(context, name), new CounterMetric());
    }

    /// <summary>
    ///     Creates a new histogram metric and registers it under the given type and name
    /// </summary>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="biased">Whether the sample type is biased or uniform</param>
    /// <returns></returns>
    public HistogramMetric Histogram(Type owner, string name, bool biased)
    {
        return GetOrAdd(new MetricName(owner, name),
            new HistogramMetric(biased
                ? HistogramMetric.SampleType.Biased
                : HistogramMetric.SampleType.Uniform));
    }

    /// <summary>
    ///     Creates a new histogram metric and registers it under the given type and name
    /// </summary>
    /// <param name="context">The context for this metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="biased">Whether the sample type is biased or uniform</param>
    /// <returns></returns>
    public HistogramMetric Histogram(string context, string name, bool biased)
    {
        return GetOrAdd(new MetricName(context, name),
            new HistogramMetric(biased
                ? HistogramMetric.SampleType.Biased
                : HistogramMetric.SampleType.Uniform));
    }

    /// <summary>
    ///     Creates a new non-biased histogram metric and registers it under the given type and name
    /// </summary>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <returns></returns>
    public HistogramMetric Histogram(Type owner, string name)
    {
        return GetOrAdd(new MetricName(owner, name), new HistogramMetric(HistogramMetric.SampleType.Uniform));
    }

    /// <summary>
    ///     Creates a new non-biased histogram metric and registers it under the given type and name
    /// </summary>
    /// <param name="context">The context for this the metric</param>
    /// <param name="name">The metric name</param>
    /// <returns></returns>
    public HistogramMetric Histogram(string context, string name)
    {
        return GetOrAdd(new MetricName(context, name), new HistogramMetric(HistogramMetric.SampleType.Uniform));
    }

    /// <summary>
    ///     Creates a new meter metric and registers it under the given type and name
    /// </summary>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="eventType">The plural name of the type of events the meter is measuring (e.g., <code>"requests"</code>)</param>
    /// <param name="unit">The rate unit of the new meter</param>
    /// <returns></returns>
    public MeterMetric Meter(Type owner, string name, string eventType, TimeUnit unit)
    {
        var metricName = new MetricName(owner, name);
        if (_metrics.TryGetValue(metricName, out var existingMetric)) return (MeterMetric)existingMetric;

        var metric = MeterMetric.New(eventType, unit);
        var justAddedMetric = _metrics.GetOrAdd(metricName, metric);
        return justAddedMetric == null ? metric : (MeterMetric)justAddedMetric;
    }

    /// <summary>
    ///     Creates a new meter metric and registers it under the given type and name
    /// </summary>
    /// <param name="context">The context for this metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="eventType">The plural name of the type of events the meter is measuring (e.g., <code>"requests"</code>)</param>
    /// <param name="unit">The rate unit of the new meter</param>
    /// <returns></returns>
    public MeterMetric Meter(string context, string name, string eventType, TimeUnit unit)
    {
        var metricName = new MetricName(context, name);
        if (_metrics.TryGetValue(metricName, out var existingMetric)) return (MeterMetric)existingMetric;

        var metric = MeterMetric.New(eventType, unit);
        var justAddedMetric = _metrics.GetOrAdd(metricName, metric);
        return justAddedMetric == null ? metric : (MeterMetric)justAddedMetric;
    }

    /// <summary>
    ///     Creates a new timer metric and registers it under the given type and name
    /// </summary>
    /// <param name="owner">The type that owns the metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="durationUnit">The duration scale unit of the new timer</param>
    /// <param name="rateUnit">The rate unit of the new timer</param>
    /// <returns></returns>
    public TimerMetric Timer(Type owner, string name, TimeUnit durationUnit, TimeUnit rateUnit)
    {
        var metricName = new MetricName(owner, name);
        if (_metrics.TryGetValue(metricName, out var existingMetric)) return (TimerMetric)existingMetric;

        var metric = new TimerMetric(durationUnit, rateUnit);
        var justAddedMetric = _metrics.GetOrAdd(metricName, metric);
        return justAddedMetric == null ? metric : (TimerMetric)justAddedMetric;
    }

    /// <summary>
    ///     Creates a new timer metric and registers it under the given type and name
    /// </summary>
    /// <param name="context">The context for this metric</param>
    /// <param name="name">The metric name</param>
    /// <param name="durationUnit">The duration scale unit of the new timer</param>
    /// <param name="rateUnit">The rate unit of the new timer</param>
    /// <returns></returns>
    public TimerMetric Timer(string context, string name, TimeUnit durationUnit, TimeUnit rateUnit)
    {
        var metricName = new MetricName(context, name);
        if (_metrics.TryGetValue(metricName, out var existingMetric)) return (TimerMetric)existingMetric;

        var metric = new TimerMetric(durationUnit, rateUnit);
        var justAddedMetric = _metrics.GetOrAdd(metricName, metric);
        return justAddedMetric == null ? metric : (TimerMetric)justAddedMetric;
    }

    /// <summary>
    ///     Enables the console reporter and causes it to print to STDOUT with the specified period
    /// </summary>
    /// <param name="period">The period between successive outputs</param>
    /// <param name="unit">The time unit of the period</param>
    public void EnableConsoleReporting(long period, TimeUnit unit)
    {
        var reporter = new ConsoleReporter(this);
        reporter.Start(period, unit);
    }

    /// <summary>
    ///     Clears all previously registered metrics
    /// </summary>
    public void Clear()
    {
        _metrics.Clear();
    }

    private T GetOrAdd<T>(MetricName name, T metric) where T : IMetric
    {
        if (_metrics.ContainsKey(name)) return (T)_metrics[name];

        var added = _metrics.AddOrUpdate(name, metric, (n, m) => m);
        return added == null ? metric : (T)added;
    }
}