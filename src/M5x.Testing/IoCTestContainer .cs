using System;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace M5x.Testing
{
    public class IoCTestContainer : IDisposable
    {
        public IoCTestContainer()
        {
            DataFactory = new Fixture();
            Services = new ServiceCollection();
            Services.AddTestHelpers();
        }

        public Fixture DataFactory { get; }
        public IServiceCollection Services { get; }

        private IServiceProvider Provider => Services?.BuildServiceProvider();
        public IApplicationBuilder App => new ApplicationBuilder(Provider);


        public T GetService<T>()
        {
            return Provider.GetService<T>();
        }

        public T GetRequiredService<T>()
        {
            return Provider.GetRequiredService<T>();
        }

        public T GetHostedService<T>()
        {
            var candidates = Provider.GetServices<IHostedService>();
            foreach (var candidate in candidates)
                if (candidate is T cand)
                    return cand;
            return default;
        }

        public IEnumerable<T> GetServices<T>()
        {
            return Provider.GetServices<T>();
        }

        public void Dispose()
        { }
    }
}