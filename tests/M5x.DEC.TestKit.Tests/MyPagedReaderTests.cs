using M5x.Config;
using M5x.DEC.TestKit.Integration.Qry;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests;

public class MyPagedReaderTests : EnumerableReaderTests<IMyCouchReader, MyPagedQry, MyReadModel>
{
    public MyPagedReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected override void Initialize()
    {
        Query = MyTestContract.PagedQry;
        Reader = Container.GetRequiredService<IMyCouchReader>();
        ;
    }

    protected override void SetTestEnvironment()
    {
        DotEnv.FromEmbedded();
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services.AddMyReader();
    }
}