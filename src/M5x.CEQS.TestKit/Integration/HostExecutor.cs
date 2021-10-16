using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace M5x.CEQS.TestKit.Integration
{
    public class HostExecutor
    {
        private readonly IEnumerable<IHostedService> _services;
        private readonly ILogger _logger;

        public HostExecutor(ILogger logger, IEnumerable<IHostedService> services)
        {
            _logger = logger;
            _services = services;
        }

        public async Task StartAsync(CancellationToken token)
        {
            try
            {
                await ExecuteAsync(service => service.StartAsync(token));
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred starting the application", ex);
            }
        }

        public async Task StopAsync(CancellationToken token)
        {
            try
            {
                await ExecuteAsync(service => service.StopAsync(token));
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred stopping the application", ex);
            }
        }

        private async Task ExecuteAsync(Func<IHostedService, Task> callback)
        {
            List<Exception> exceptions = null;

            foreach (var service in _services)
            {
                try
                {
                    await callback(service);
                }
                catch (Exception ex)
                {
                    exceptions ??= new List<Exception>();

                    exceptions.Add(ex);
                }
            }
            // Throw an aggregate exception if there were any exceptions
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}