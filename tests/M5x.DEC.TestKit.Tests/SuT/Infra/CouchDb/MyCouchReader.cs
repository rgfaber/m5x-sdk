using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Infra;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb;

public static class Inject
{
    public static IServiceCollection AddMyReader(this IServiceCollection services)
    {
        return services
            .AddConsoleLogger()
            .AddCouchClient()
            .AddMyDb()
            .AddTransient<IMyCouchReader, MyCouchReader>();
        ;
    }


    public static IServiceCollection AddMyWriter(this IServiceCollection services)
    {
        return services
            .AddConsoleLogger()
            .AddCouchClient()
            .AddMyDb()
            .AddTransient<IMyCouchWriter, MyCouchWriter>();
        ;
    }
}

public interface IMyCouchReader : IModelReader<MyPagedQry, MyReadModel>
{
}

public class MyCouchReader : CouchReader<MyPagedQry, MyReadModel>, IMyCouchReader
{
    public MyCouchReader(IMyCouchDb couchDb, ILogger logger) : base(couchDb, logger)
    {
    }

    public override async Task<IEnumerable<MyReadModel>> FindAllAsync(MyPagedQry pagedQry)
    {
        return await CouchDb.RetrieveRecent(pagedQry.PageNumber, pagedQry.PageSize);
    }
}