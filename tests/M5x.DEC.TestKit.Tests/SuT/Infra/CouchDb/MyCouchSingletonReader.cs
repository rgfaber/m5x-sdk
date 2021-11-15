using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb
{
    public interface IMyCouchSingletonReader : ISingleModelReader<MySingletonQuery, MyReadModel>
    {
    }

    internal class MyCouchSingletonReader : CouchReader<MySingletonQuery, MyReadModel>, IMyCouchSingletonReader
    {
        public MyCouchSingletonReader(IMyCouchDb couchDb, ILogger logger) : base(couchDb, logger)
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