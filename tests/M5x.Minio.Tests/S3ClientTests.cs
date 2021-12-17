using System;
using System.Threading.Tasks;
using M5x.Minio.Interfaces;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Minio.Tests;

public class S3ClientTests : IoCTestsBase
{
    private IS3Client _s3;

    public S3ClientTests(ITestOutputHelper output, IoCTestContainer container)
        : base(output, container)
    {
    }

    protected override void Initialize()
    {
        _s3 = Container.GetService<IS3Client>();
    }

    protected override void SetTestEnvironment()
    {
        Environment.SetEnvironmentVariable(EnVars.S3_ENDPOINT, "http://s3.local");
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services
            .AddS3Client(S3Config.Endpoint,
                S3Config.PublicKey,
                S3Config.PrivateKey,
                S3Config.Region,
                S3Config.SessionToken);
    }


    [Fact]
    public async Task Try_CreateBucket()
    {
        Assert.NotNull(_s3);
        await _s3.MakeBucketAsync("test-bucket");
    }
}