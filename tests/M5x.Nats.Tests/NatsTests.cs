using System;
using System.Threading.Tasks;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Nats.Tests;

public class NatsTests : NatsTestsBase
{
    public NatsTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected override void SetTestEnvironment()
    {
        Environment.SetEnvironmentVariable(EnVars.NATS_HOST, "nats.local");
        Environment.SetEnvironmentVariable(EnVars.NATS_PORT, "4222");
        Environment.SetEnvironmentVariable(EnVars.NATS_USER, "test");
        Environment.SetEnvironmentVariable(EnVars.NATS_PWD, "T0pS3cr3t");
    }

    [Fact]
    public async Task Should_PublishMessages()
    {
        try
        {
            if (!Client.IsConnected) await Client.ConnectAsync();
            await Client.PubManyAsync(async publisher =>
            {
                var x = 0;
                do
                {
                    x++;
                    await publisher.PubAsync("myTestSubject", $"Message at {DateTime.UtcNow}");
                } while (x <= 10000);
            });
            ;
        }
        catch (Exception e)
        {
            Output.WriteLine(e.Message);
            throw;
        }
    }
}