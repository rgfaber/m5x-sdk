using M5x.DEC.Schema;
using M5x.DEC.TestKit.Unit.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Game.Schema;
using Xunit.Abstractions;

namespace Robby.Schema.UnitTests
{
    public static class Aggregate
    {
        public class GameIdTests : IDTests<GameModel.ID>
        {
            public GameIdTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }

            protected override void SetTestEnvironment()
            {
            }

            protected override void InjectDependencies(IServiceCollection services)
            {
            }

            protected override GameModel.ID NewID()
            {
                return Identity<GameModel.ID>.New;
            }
        }
    }
}