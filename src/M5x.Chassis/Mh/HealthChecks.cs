using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using M5x.Chassis.Mh.Core;

namespace M5x.Chassis.Mh;

/// <summary> A manager class for health checks </summary>
public class HealthChecks : IDisposable
{
    private readonly ConcurrentDictionary<string, IHealthCheck> _checks =
        new();

    /// <summary> Returns <code>true</code> <see cref="HealthCheck" />s have been registered, <code>false</code> otherwise </summary>
    public bool HasHealthChecks => !_checks.IsEmpty;

    public void Dispose()
    {
        foreach (var check in _checks)
            using (check.Value as IDisposable)
            {
            }
    }

    /// <summary>
    ///     Registers an application <see cref="HealthCheck" /> with a given name
    /// </summary>
    /// <param name="name">The named health check instance</param>
    /// <param name="check">The <see cref="HealthCheck" /> function</param>
    public void Register(string name, Func<HealthCheck.Result> check)
    {
        var healthCheck = new HealthCheck(name, check);
        if (!_checks.ContainsKey(healthCheck.Name)) _checks.TryAdd(healthCheck.Name, healthCheck);
    }

    /// <summary> Runs the registered health checks and returns a map of the results as a metrics container. </summary>
    public IDictionary<MetricName, IMetric> RunHealthChecks()
    {
        var results = new SortedDictionary<MetricName, IMetric>();
        foreach (var entry in _checks)
        {
            var result = entry.Value.Execute();
            results.Add(new MetricName(typeof(HealthCheck), entry.Key), result);
        }

        return results;
    }
}