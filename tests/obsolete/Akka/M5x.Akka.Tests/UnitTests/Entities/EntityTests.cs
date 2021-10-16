using System;
using FluentAssertions;
using M5x.Akka.TestHelpers.Aggregates.Entities;
using Xunit;

namespace M5x.Akka.Tests.UnitTests.Entities
{
    public class EntityTests
    {
        [Fact]
        public void InstantiatingEntity_WithNullId_ThrowsException()
        {
            this.Invoking(test => new Test(null))
                .Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void InstantiatingEntity_WithValidId_HasIdentity()
        {
            var testId = TestId.New;
            
            var test = new Test(testId);

            test.GetIdentity().Should().Be(testId);
        }
    }
}