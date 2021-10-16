using System.Collections.Generic;
using System.Threading.Tasks;
using CouchDB.Driver;
using FakeItEasy;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{

    
    public interface IMySingletonReader: ISingleModelReader<MySingletonQuery, MyReadModel> {}

    internal class MySingletonReader : CouchReader<MySingletonQuery, MyReadModel>, IMySingletonReader
    {
        public MySingletonReader(IMyDb db, ILogger logger) : base(db, logger)
        {
        }

        public override async Task<IEnumerable<MyReadModel>> FindAllAsync(MySingletonQuery qry)
        {
            var res = new MyReadModel[1];
            res[0] = await GetByIdAsync(qry.Id);
            return res;
        }
    }
}