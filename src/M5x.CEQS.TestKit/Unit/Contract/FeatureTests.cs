using System.Threading.Tasks;
using EventFlow.Core;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Utils;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Unit.Contract
{
        public abstract class FeatureTests<TAggregateId, TFact, THope, TFeedback> : IoCTestsBase
        where TAggregateId: IIdentity
        where TFact: IFact<TAggregateId>
        where THope: IHope<TAggregateId>
        where TFeedback: IFeedback<TAggregateId>
        {
            protected TFact Fact;
            protected TAggregateId Id;
            protected THope Hope;
            protected string CorrelationId;
            protected TFeedback Feedback;

            public FeatureTests(ITestOutputHelper output, 
                IoCTestContainer container) : base(output, container)
            {
            }

            [Fact]
            public void Must_HaveHope()
            {
                Assert.NotNull(Hope);
            }

            [Fact]
            public void Must_HaveFact()
            {
                Assert.NotNull(Fact);
            }

            [Fact]
            public void Must_HaveID()
            {
                Assert.NotNull(Id);
            }

            [Fact]
            public void Must_HaveCorrelationId()
            {
                Assert.False(string.IsNullOrWhiteSpace(CorrelationId));
            }

            [Fact]
            public void Must_HaveFeedback()
            {
                Assert.NotNull(Feedback);
            }

            protected override async void Initialize()
            {
                CorrelationId = GuidUtils.NewCleanGuid;
                Id = CreateId();
                Hope = CreateHope(Id, CorrelationId);
                Fact = CreateFact(Id, CorrelationId);
                Feedback  = CreateFeedback(Id, CorrelationId);
            }

            protected abstract TFeedback CreateFeedback(TAggregateId id, string correlationId);
            protected abstract TFact CreateFact(TAggregateId id, string correlationId);
            protected abstract THope CreateHope(TAggregateId id,string correlationId);
            protected abstract TAggregateId CreateId();
        }
}