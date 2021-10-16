using System;
using FluentAssertions;
using M5x.Akka.Core;
using Xunit;

namespace M5x.Akka.Tests.UnitTests.Core
{
    public class SourceIdTests
    {
        [Fact]
        public void InstantiatingSourceId_WithNullString_ThrowsException()
        {
            this.Invoking(test => new SourceId(null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}