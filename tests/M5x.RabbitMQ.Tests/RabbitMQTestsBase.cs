using M5x.Testing;
using Xunit.Abstractions;

namespace M5x.RabbitMQ.Tests;

public abstract class RabbitMQTestsBase : IoCTestsBase
{
    public RabbitMQTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }
}