using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Serilog;

namespace M5x.DEC.Infra.CouchDb;

public abstract class CouchStore<TStateModel, TId> : CouchStoreBase<TStateModel, TId>
    where TId : IIdentity
    where TStateModel : IStateEntity<TId>
{
    protected CouchStore(ICouchClient client, ILogger logger) : base(client, logger)
    {
    }

    protected CouchStore(string dbName,
        string connectionString,
        Action<CouchOptionsBuilder> couchSettingsFunc,
        Action<ClientFlurlHttpSettings> flurlSettingsFunc) : base(dbName,
        connectionString,
        couchSettingsFunc,
        flurlSettingsFunc)
    {
    }

    protected CouchStore(ICouchClient clt,
        ILogger logger,
        ICouchDatabase<CDoc<TStateModel>> cachedDb = null) : base(clt,
        logger,
        cachedDb)
    {
    }

    public override async Task<TStateModel> AddOrUpdateAsync(TStateModel entity, bool batch = false,
        bool withConflicts = false, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(entity, nameof(entity));
        Guard.Against.NullOrWhiteSpace(entity.Id, nameof(entity.Id));
        TStateModel res;
        var doc = CreateCDoc(entity);
        try
        {
            var oldDoc = await Db.FindAsync(doc.Id, withConflicts, cancellationToken);
            if (oldDoc != null)
                if (oldDoc.Rev != entity.Prev)
                    doc.Rev = oldDoc.Rev;
            doc = await Db.AddOrUpdateAsync(doc, batch, cancellationToken);
            res = doc.Data;
            res.Prev = doc.Rev;
        }
        catch (Exception e)
        {
            Logger?.Error(JsonSerializer.Serialize(e.AsApiError()));
            throw;
        }

        return res;
    }
}