using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC.Infra.CouchDb;

public abstract class CouchReader<TQuery, TReadModel> : IModelReader<TQuery, TReadModel>
    where TQuery : Query
    where TReadModel : IReadEntity
{
    protected readonly ICouchStore<TReadModel> CouchDb;
    protected readonly ILogger Logger;

    public CouchReader(ICouchStore<TReadModel> couchDb, ILogger logger)
    {
        CouchDb = couchDb;
        Logger = logger;
    }


    public abstract Task<IEnumerable<TReadModel>> FindAllAsync(TQuery qry);


    public async Task<TReadModel> GetByIdAsync(string id)
    {
        return await CouchDb.GetByIdAsync(id);
    }
}