using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.Config;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Etl.Infra.Game;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Etl.Infra.IntegrationTests.Game
{
    public class StoreTests : IoCTestsBase
    {
        private readonly string _id = "robby-93c34455-4f0d-4a94-8fdc-090866156153";
        private Schema.Game _game;
        private IGameDb _gameStore;


        private IEnumerable<Schema.Game> _sims;


        public StoreTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public async Task Must_StoreReturnRobbySimulation()
        {
            _game = await _gameStore.GetByIdAsync(_id);
            Assert.NotNull(_game);
        }

        [Fact]
        public async Task Must_FindMany()
        {
            _sims = await _gameStore.RetrieveRecent(1, 20);
            Assert.NotNull(_sims);
        }


        [Fact]
        public void Needs_Store()
        {
            Assert.NotNull(_gameStore);
        }

        protected override void Initialize()
        {
            _gameStore = Container.GetService<IGameDb>();
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddRobbyEtlInfra();
        }
    }
}