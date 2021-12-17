using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace M5x.Testing;

public interface IHostExecutor
{
    Task StartAsync(CancellationToken token = default);
    Task StopAsync(CancellationToken token = default);
}

public class HostExecutor : IHostExecutor
{
    private readonly ILogger _logger;
    private readonly IEnumerable<IHostedService> _services;

    public HostExecutor(ILogger logger, IEnumerable<IHostedService> services)
    {
        _logger = logger;
        _services = services;
    }

    public Task StartAsync(CancellationToken token)
    {
        try
        {
            ExecuteAsync(service => service.StartAsync(token));
        }
        catch (Exception ex)
        {
            _logger?.Error("An error occurred starting the application", ex);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token)
    {
        try
        {
            ExecuteAsync(service => service.StopAsync(token));
        }
        catch (Exception ex)
        {
            _logger?.Error("An error occurred stopping the application", ex);
        }

        return Task.CompletedTask;
    }

    private Task ExecuteAsync(Func<IHostedService, Task> callback)
    {
        List<Exception> exceptions = null;

        foreach (var service in _services)
            try
            {
                callback(service);
            }
            catch (Exception ex)
            {
                exceptions ??= new List<Exception>();

                exceptions.Add(ex);
            }

        // Throw an aggregate exception if there were any exceptions
        if (exceptions != null) throw new AggregateException(exceptions);
        return Task.CompletedTask;
    }
}