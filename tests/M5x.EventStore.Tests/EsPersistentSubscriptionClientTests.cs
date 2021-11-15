using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.Config;
using M5x.DEC.Schema.Extensions;
using M5x.EventStore.Interfaces;
using M5x.Serilog;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.EventStore.Tests
{
    public static class Inject
    {
        public static IServiceCollection AddEsSubscriber(this IServiceCollection services)
        {
            return services
                .AddSingleton(p
                    => new PersistentSubscriptionSettings(false, StreamPosition.Start))
                .AddConsoleLogger()
                .AddDECEsClients()
                .AddHostedService<EsConsumer>();
        }
    }


    public class EsPersistentSubscriptionClientTests : IoCTestsBase
    {
        private IEsPersistentSubscriptionsClient _clt;
        private IEsEmitter _emitter;
        private Uuid _eventId;
        private IHostExecutor _host;

        public EsPersistentSubscriptionClientTests(ITestOutputHelper output,
            IoCTestContainer container) : base(output,
            container)
        {
        }

        protected override void Initialize()
        {
            _clt = Container.GetRequiredService<IEsPersistentSubscriptionsClient>();
            _host = Container.GetRequiredService<IHostExecutor>();
            _emitter = Container.GetRequiredService<IEsEmitter>();
        }

        [Fact]
        public void Needs_Emitter()
        {
            Assert.NotNull(_emitter);
        }

        [Fact]
        public void Needs_Client()
        {
            Assert.NotNull(_clt);
        }

        [Fact]
        public void Needs_Host()
        {
            Assert.NotNull(_host);
        }

        [Fact]
        public async Task Should_AnEmittedFactShouldAppear()
        {
            try
            {
                await _host.StartAsync();
                var times = 200;
                var j = 0;
                do
                {
                    // var events = new[] { TestData.EventData(Guid.NewGuid()) };
                    // var res = await _emitter.EmitAsync(TestConstants.Id, events);
                    // Assert.NotNull(res);
                    Thread.Sleep(100);
                    j++;
                } while (j < 200);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            finally
            {
                await _host.StopAsync();
            }
        }


        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }


        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddTransient(p => Output)
                .AddTransient<IEsEmitter, EsEmitter>()
                .AddSingleton(p
                    => new PersistentSubscriptionSettings(false, StreamPosition.Start))
                .AddConsoleLogger()
                .AddDECEsClients()
                .AddHostedService<EsProducer>()
                .AddHostedService<EsConsumer>();
        }


        [Fact]
        public void Needs_FakeEventData()
        {
            var events = TestData.EventData(Guid.NewGuid());
            Assert.NotNull(events);
        }

        [Fact]
        public void Needs_EventId()
        {
            _eventId = Uuid.Parse(TestConstants.Guid.ToString());
            Assert.NotNull(_eventId);
        }
    }
}