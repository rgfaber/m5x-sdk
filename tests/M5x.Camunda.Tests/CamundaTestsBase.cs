using System;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.Camunda.Tests
{
    public abstract class CamundaTestsBase : IoCTestsBase
    {
        protected CamundaTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void SetTestEnvironment()
        {
            Environment.SetEnvironmentVariable(EnVars.CAMUNDA_URL, "http://localhost:8080/engine-rest");
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services?
                .AddBpm();
        }
    }
}