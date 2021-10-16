using System;
using FluentAssertions;
using M5x.Akka.Commands;
using M5x.Akka.TestHelpers.Aggregates;
using M5x.Akka.TestHelpers.Aggregates.Commands;
using Xunit;

namespace M5x.Akka.Tests.UnitTests.Commands
{
    public class CommandTests
    {
        [Fact]
        public void InstantiatingCommand_WithValidInput_ThrowsException()
        {
            var aggregateId = TestAggregateId.New;
            var sourceId = CommandId.New;
            
            var command = new CreateTestCommand(aggregateId, sourceId);

            command.GetSourceId().Should().Be(sourceId);
        }
        
        [Fact]
        public void InstantiatingCommand_WithNullId_ThrowsException()
        {
            this.Invoking(test => new CreateTestCommand(null, CommandId.New))
                .Should().Throw<ArgumentNullException>().And.Message.Contains("aggregateId").Should().BeTrue();
        }
        
        [Fact]
        public void InstantiatingCommand_WithNullSourceId_ThrowsException()
        {
            this.Invoking(test => new CreateTestCommand(TestAggregateId.New, null))
                .Should().Throw<ArgumentNullException>().And.Message.Contains("sourceId").Should().BeTrue();
        }
    }
}