using M5x.Config;
using M5x.DEC.TestKit.Integration.Etl;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests;

public class
    MyCouchWriterTests : FactWriterTests<IMyCouchWriter, IMyCouchReader, MyID, MyFact, MyReadModel, MyPagedQry>
{
    public MyCouchWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected override void Initialize()
    {
        Fact = MyTestContract.Fact;
        Query = MyTestContract.PagedQry;
        Reader = Container.GetRequiredService<IMyCouchReader>();
        Writer = Container.GetRequiredService<IMyCouchWriter>();
    }

    protected override void SetTestEnvironment()
    {
        DotEnv.FromEmbedded();
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services
            .AddMyReader()
            .AddMyWriter();
    }
}