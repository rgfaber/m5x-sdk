// ***********************************************************************
// <copyright file="Add.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************


using M5x.Chassis.Service.Container;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Service.Metrics;

/// <summary>
///     Class Add.
/// </summary>
public static partial class Add
{
    /// <summary>
    ///     Adds the hq.
    /// </summary>
    /// <param name="services">The services.</param>
    public static IServiceCollection AddMh(this IServiceCollection services)
    {
        RegisterMetricsAndHealthChecks(services.AddContainer());
        return services;
    }
}