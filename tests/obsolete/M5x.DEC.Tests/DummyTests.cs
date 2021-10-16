using M5x.Schemas;
using M5x.Schemas.Commands;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.Tests
{
    public class DummyTests : IoCTestsBase
    {
        public DummyTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public void Must_AggregateIdMustBeInNameGuidFormat()
        {
            var root = DummyRoot.CreateNew();
            Assert.NotNull(root);
            Assert.NotNull(root.Id);
        }

        [Fact]
        public void Must_BeAbleToCreateCommands()
        {
            var cmd = Cmd.CreateNew(TestId.NewComb());
            Assert.NotNull(cmd);
        }


        protected override void Initialize()
        {
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }


        [IDPrefix("dummy")]
        public record DummyID : AggregateId<DummyID>
        {
            public DummyID(string id) : base(id)
            {
            }
        }

        public class DummyRoot : AggregateRoot<DummyID>
        {
            private DummyRoot(DummyID id) : base(id)
            {
            }

            public static DummyRoot CreateNew()
            {
                var id = DummyID.NewComb();
                return new DummyRoot(id);
            }
        }

        [IDPrefix("test")]
        private record TestId : AggregateId<TestId>
        {
            public TestId(string value) : base(value)
            {
            }
        }

        private class TestAggregate : AggregateRoot<TestId>
        {
            public TestAggregate(TestId id) : base(id)
            {
            }
        }

        [Topic("testCmd")]
        private record Cmd : Command<TestId>
        {
            public Cmd(TestId aggregateId) : base(aggregateId)
            {
            }

            public Cmd(TestId aggregateId, CommandID sourceId) : base(aggregateId, sourceId)
            {
            }

            public static Cmd CreateNew(TestId id)
            {
                return new(id);
            }
        }
    }
}