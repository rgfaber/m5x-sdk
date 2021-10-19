using System;
using System.Threading.Tasks;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.RabbitMQ.Tests
{
    public abstract class RabbitMQTestsBase : IoCTestsBase
    {
        public RabbitMQTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
    }
}