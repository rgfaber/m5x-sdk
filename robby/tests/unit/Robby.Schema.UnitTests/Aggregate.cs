using M5x.DEC.Schema;
using M5x.DEC.TestKit.Unit.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Robby.Schema.UnitTests
{
    public static class Aggregate
    {
        public class GameIdTests : IDTests<Schema.Game.ID>
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

            protected override Schema.Game.ID NewID()
            {
                return Identity<Schema.Game.ID>.New;
            }
        }
    }
}