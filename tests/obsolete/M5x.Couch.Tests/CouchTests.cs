using System;
using System.Threading.Tasks;
using M5x.Couch.Tests.Interfaces;
using M5x.Docker;
using M5x.Store;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Couch.Tests
{
    public class CouchTests : IoCTestsBase, ICouchTests
    {
        public CouchTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Try_StartCouch()
        {
            // var couch = Container.GetService<ICouchContainer>();
            // Assert.NotNull(couch);
            // var res = couch.Start(Library.CouchDb).Result;
            // Assert.NotNull(res);
        }

        protected override void Initialize()
        {
            // Try_StartCouch();
        }

        protected override void SetTestEnvironment()
        {
            StoreConfig.BuildEnvironment();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddDockerEnvironment()
                .AddCouchContainer();
        }
    }
}