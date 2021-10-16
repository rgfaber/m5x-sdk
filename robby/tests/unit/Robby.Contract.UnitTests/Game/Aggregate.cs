using M5x.DEC.Schema;
using M5x.DEC.TestKit.Unit.Contract;
using M5x.Testing;
using Xunit.Abstractions;

namespace Robby.Contract.UnitTests.Game
{
    public static class Aggregate
    {
        public abstract class FeatureTests<TFact, THope, TFeedback> :
            FeatureTests<Schema.Game.ID, TFact, THope, TFeedback>
            where TFeedback : IFeedback
            where THope : IHope
            where TFact : IFact
        {
            protected FeatureTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }
        }


        public abstract class
            QueryTests<TQuery, TResponse> : M5x.DEC.TestKit.Unit.Contract.QueryTests<TQuery, TResponse>
            where TResponse : IResponse
            where TQuery : IQuery
        {
            protected QueryTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }
        }
    }
}