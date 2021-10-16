using System.Collections.Generic;
using System.Threading.Tasks;
using CouchDB.Driver;
using FakeItEasy;
using M5x.DEC.Infra;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static partial class Inject
    {
        public static IServiceCollection AddMyReader(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddCouchClient()
                .AddMyDb()
                .AddTransient<IMyReader, MyReader>(); ;
        }
        
        public static IServiceCollection AddMyWriter(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddCouchClient()
                .AddMyDb()
                .AddTransient<IMyWriter, MyWriter>(); ;
        }
    }
    
    
    public interface IMyReader: IModelReader<MyPagedQry,MyReadModel> {}
    
    public class MyReader: CouchReader<MyPagedQry, MyReadModel>, IMyReader
    {
        public MyReader(IMyDb db, ILogger logger) : base(db, logger)
        {
        }

        public override async Task<IEnumerable<MyReadModel>> FindAllAsync(MyPagedQry pagedQry)
        {
            return await Db.RetrieveRecent(pagedQry.PageNumber, pagedQry.PageSize);
        }
    }
}