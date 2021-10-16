using M5x.DEC.TestKit.Unit.Domain;
using M5x.Testing;
using Xunit.Abstractions;

namespace Robby.Domain.UnitTests.Game
{
    public static class Aggregate
    {
        public abstract class GameFeatureTests : FeatureTests<
            Domain.Game.Aggregate.Root,
            Schema.Game.ID,
            Schema.Game,
            Domain.Game.Initialize.Cmd,
            Domain.Game.Initialize.Evt,
            Contract.Game.Features.Initialize.Hope,
            Contract.Game.Features.Initialize.Feedback,
            Contract.Game.Features.Initialize.Fact>
        {
            protected GameFeatureTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }
        }
    }
}