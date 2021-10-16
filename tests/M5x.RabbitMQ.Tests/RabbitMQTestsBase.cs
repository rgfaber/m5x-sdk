using System;
using System.Threading.Tasks;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.RabbitMQ.Tests
{
    public class RabbitMQTestsBase : IoCTestsBase
    {
        public RabbitMQTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override void SetTestEnvironment()
        {
            throw new NotImplementedException();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}