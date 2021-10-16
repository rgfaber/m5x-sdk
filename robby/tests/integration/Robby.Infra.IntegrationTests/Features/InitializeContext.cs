using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.Schema;
using M5x.DEC.TestKit.Integration;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Client.Infra;
using Robby.Cmd.Infra;
using Robby.Cmd.Infra.Game;
using Robby.Etl.Infra;
using Robby.Etl.Infra.Game;
using Robby.Qry.Infra;
using Robby.Qry.Infra.Game;
using Robby.Schema;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Infra.IntegrationTests.Features
{
    public static class InitializeContext
    {
        public class IntegrationTests : IoCTestsBase
        {
            private Game.ID _gameId;

            protected ById.IReader Reader;

//            protected Domain.InitializeContext.Handler Handler;
            protected Client.Infra.Features.Initialize.IRequester Requester;

            protected Cmd.Infra.Game.InitializeGame.Responder Responder;

            protected Domain.Game.Aggregate.Root Root;
            protected Etl.Infra.Game.Initialize.Subscriber Subscriber;
            protected Etl.Infra.Game.Initialize.IWriter Writer;


            public IntegrationTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }


            [Fact]
            public void Needs_Root()
            {
                Assert.NotNull(Root);
            }



            [Fact]
            public void Needs_Subscriber()
            {
                Assert.NotNull(Subscriber);
            }

            [Fact]
            public void Needs_Writer()
            {
                Assert.NotNull(Writer);
            }


            [Fact]
            public void Needs_Reader()
            {
                Assert.NotNull(Reader);
            }


            [Fact]
            public void Needs_Requester()
            {
                Assert.NotNull(Requester);
            }


            [Fact]
            public void Needs_Responder()
            {
                Assert.NotNull(Responder);
            }


            protected override async void Initialize()
            {
                try
                {
                    _gameId = CreateId();
                    Root = CreateAggregateRoot();
                    Responder = Container.GetHostedService<Cmd.Infra.Game.InitializeGame.Responder>();
                    Requester = Container.GetService<Client.Infra.Features.Initialize.IRequester>();
                    Writer = Container.GetService<Etl.Infra.Game.Initialize.IWriter>();
                    Subscriber = Container.GetHostedService<Etl.Infra.Game.Initialize.Subscriber>();
                    Reader = Container.GetService<ById.IReader>();
                    await StartResponder();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    throw;
                }
            }

            protected async Task StartResponder()
            {
                var executor = Container.GetService<HostExecutor>();
                await executor.StartAsync(CancellationToken.None);
            }

            protected Game.ID CreateId()
            {
                return Identity<Game.ID>.New;
            }

            protected Domain.Game.Aggregate.Root CreateAggregateRoot()
            {
                return Domain.Game.Aggregate.Root.New(_gameId);
            }

            protected override void SetTestEnvironment()
            {
                DotEnv.FromEmbedded();
            }

            protected override void InjectDependencies(IServiceCollection services)
            {
                services
                    .AddSingleton<HostExecutor>()
                    .AddRobbyCmdInfra()
                    .AddRobbyRequesters()
                    .AddRobbyEtlInfra()
                    .AddRobbyQryInfra();
            }

            [Fact]
            public async Task
                Given_ExpressHopeInitializeContext_When_ContextInitializedFactProcessed_Then_StorageMustContainInitialRobbySimulation()
            {
                var feedback =
                    await Requester.RequestAsync(Contract.Game.Features.Initialize.Hope.New(_gameId.Value,
                        InitializationOrder.New("Test", 500, 200, 344, 50)));
                Assert.NotNull(feedback);
                Assert.True(feedback.IsSuccess);
            }
        }
    }
}