using System;
using EventFlow;
using EventFlow.DependencyInjection.Extensions;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.EventFlow;

public static class Inject
{
    public static IServiceCollection AddCEQS(this IServiceCollection services, Action<IEventFlowOptions> options)
    {
        return services
            .AddConsoleLogger()
            .AddEventFlow(options);
    }
}