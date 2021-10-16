using System.Threading.Tasks;
using M5x.CEQS.TestKit.Integration;
using M5x.CEQS.TestKit.Integration.Containers;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Docker.Lib.Tests
{
    public class ContactsTests : IoCTestsBase
    {
        public ContactsTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddCouchContainer()
                .AddDockerEnvironment();
        }


        [Fact]
        public async Task Try_StartCouchDbContainer()
        {
            var couch = Container.GetService<ICouchContainer>();
            Assert.NotNull(couch);
            var contacts = await couch.Start(Library.CouchDb);
            Assert.NotNull(contacts);
        }
    }
}