using System;
using System.Collections.Generic;
using M5x.DEC;
using M5x.EventStore;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Cmd.Infra.Game;
using Robby.Domain;
using Robby.Domain.Game;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using Aggregate = Robby.Schema.Aggregate;
using InitializeGame = Robby.Cmd.Infra.Game.InitializeGame;

namespace Robby.Cmd.Infra.IntegrationTests
{
    public static class InitializeContext
    {
        public class Integration : IoCTestsBase
        {
            private Initialize.IActor _actor;
            private IEsClient _eventStore;
            private IGameStream _eventStream;
            private IEnumerable<IEventHandler<Schema.Game.ID, Initialize.Evt>> _handlers;
            private ILogger _logger;
            private IEncodedConnection _nats;
            private InitializeGame.Responder _responder;

            public Integration(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }


            [Fact]
            public void Needs_Responder()
            {
                Assert.Null(_responder);
            }


            [Fact]
            public void Needs_Handlers()
            {
                Assert.NotNull(_handlers);
            }


            [Fact]
            public void Needs_Actor()
            {
                Assert.Null(_actor);
            }


            [Fact]
            public void Must_HaveEventRepo()
            {
                Assert.NotNull(_eventStream);
            }


            protected override void Initialize()
            {
                try
                {
                    _logger = Container.GetService<ILogger>();
                    _nats = Container.GetService<IEncodedConnection>();
                    _eventStore = Container.GetService<IEsClient>();
                    _eventStream = Container.GetService<IGameStream>();
                    _actor = Container.GetService<Domain.Game.Initialize.IActor>();
                    _responder = Container.GetHostedService<InitializeGame.Responder>();
                    _handlers = Container.GetServices<IEventHandler<Schema.Game.ID, Initialize.Evt>>();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            protected override void SetTestEnvironment()
            {
            }

            [Fact]
            public void Needs_Logger()
            {
                Assert.Null(_logger);
            }


            [Fact]
            public void Needs_EncodedConnection()
            {
                Assert.NotNull(_nats);
            }

            [Fact]
            public void Needs_EventStore()
            {
                Assert.NotNull(_eventStore);
            }


            protected override void InjectDependencies(IServiceCollection services)
            {
                services.AddRobbyCmdInfra();
            }
        }
    }
}