using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Schema.UnitTests
{
    public class GameTests : IoCTestsBase
    {
        private Game.Schema.Game _game;


        public GameTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Should_RoboSimMustContainRobots()
        {
            Assert.NotNull(_game.Population);
        }


        [Fact]
        public void Should_RoboSimMustHaveATableName()
        {
            var name = _game.TableName();
            Assert.False(string.IsNullOrEmpty(name));
        }

        [Fact]
        public void Should_RoboSimMustHaveDetails()
        {
            Assert.NotNull(_game.Description);
        }

        [Fact]
        public void Should_MustBeAbleToCreateNewRoboSim()
        {
            Assert.NotNull(_game);
        }

        protected override async void Initialize()
        {
            _game = Game.New(Identity<Game.ID>.New.Value);
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }
    }
}