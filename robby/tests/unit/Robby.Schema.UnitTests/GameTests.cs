using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Game.Schema;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Schema.UnitTests
{
    public class GameTests : IoCTestsBase
    {
        private Game.Schema.GameModel _gameModel;


        public GameTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Should_RoboSimMustContainRobots()
        {
            Assert.NotNull(_gameModel.Population);
        }


        [Fact]
        public void Should_RoboSimMustHaveATableName()
        {
            var name = _gameModel.TableName();
            Assert.False(string.IsNullOrEmpty(name));
        }

        [Fact]
        public void Should_RoboSimMustHaveDetails()
        {
            Assert.NotNull(_gameModel.Description);
        }

        [Fact]
        public void Should_MustBeAbleToCreateNewRoboSim()
        {
            Assert.NotNull(_gameModel);
        }

        protected override async void Initialize()
        {
            _gameModel = GameModel.New(Identity<GameModel.ID>.New.Value);
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }
    }
}