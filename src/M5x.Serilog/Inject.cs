using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace M5x.Serilog;

public static class Inject
{
    public static IServiceCollection AddConsoleLogger(this IServiceCollection services, bool enableSelfLog = false)
    {
        return services?
            .AddSingleton(x => CreateSeriLogConsoleLogger(enableSelfLog));
    }

    public static IServiceCollection AddElasticSearchLogger(this ServiceCollection services,
        ElasticsearchSinkOptions options = null,
        bool enableSelfLog = false)
    {
        return services?
            .AddSingleton(x => CreateSeriLogElasticSearchLogger(options, enableSelfLog));
    }


    private static ILogger CreateSeriLogConsoleLogger(bool enableSelfLog = false)
    {
        if (enableSelfLog)
        {
            SelfLog.Enable(msg => Debug.WriteLine(msg));
            SelfLog.Enable(Console.Error);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.ControlledBy(new EnvLogLevelSwitch(EnVars.LOG_LEVEL_MIN))
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .WriteTo.Console(LogEventLevel.Verbose,
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message} [{Properties}]{NewLine}{Exception}")
            .CreateLogger();
        return Log.Logger;
    }

    /// <summary>
    ///     Adds Serilog for Elasticsearch.
    ///     Please visit https://github.com/serilog/serilog-aspnetcore for more info
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="enableSelfLog"></param>
    /// <returns></returns>
    private static ILogger CreateSeriLogElasticSearchLogger(ElasticsearchSinkOptions options = null,
        bool enableSelfLog = false)
    {
        if (enableSelfLog)
        {
            SelfLog.Enable(msg => Debug.WriteLine(msg));
            SelfLog.Enable(Console.Error);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.ControlledBy(new EnvLogLevelSwitch(EnVars.LOG_LEVEL_MIN))
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .WriteTo.Elasticsearch(options)
            .CreateLogger();
        return Log.Logger;
    }
}