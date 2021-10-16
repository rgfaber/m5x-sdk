using System.Threading.Tasks;
using M5x.DEC.Infra.EventStore;
using M5x.DEC.Persistence.EventStore;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.Infra.Tests
{
    public class EventStoreDbTests: IoCTestsBase 
    {
        
        private IEventStore _store;
        
        private TestAggregateId _theId = TestAggregateId.NewComb("057410D8-AE55-45CA-8A5B-AA4DC6EB51F7");

        [Fact]
        public async Task Should_AppendEvent()
        {
                var evt = Tested.CreateNew(_theId);
                var res = await _store.AppendEventAsync<TestAggregateId>(evt);
                Assert.NotNull(res);
        }

        [Fact]
        public async Task Should_ReadEvents()
        {
            var res = await _store.ReadEventsAsync(_theId);
            Assert.NotNull(res);
        }

        public EventStoreDbTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            _store = Container.GetService<IEventStore>();
            Assert.NotNull(_store);
        }

        protected override void SetTestEnvironment()
        {
            // Environment.SetEnvironmentVariable(M5x.EventStore.EnVars.EVENTSTORE_URI, "esdb://es.local:2113");
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddSingletonEventStore();
        }
    }
}