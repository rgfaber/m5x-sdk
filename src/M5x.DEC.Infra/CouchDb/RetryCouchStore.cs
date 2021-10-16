using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CouchDB.Driver;
using CouchDB.Driver.Indexes;
using CouchDB.Driver.Options;
using CouchDB.Driver.Types;
using Flurl.Http.Configuration;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Polly;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.Infra.CouchDb
{
    public abstract class RetryCouchStore<TStateModel, TId> : CouchStoreBase<TStateModel, TId>, ICouchStore<TStateModel>
        where TStateModel : IStateEntity<TId> 
        where TId : IIdentity
    {
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly AsyncRetryPolicy _retryPolicy;
        private int _backoff = 100;

        public override Task<TStateModel> AddOrUpdateAsync(TStateModel entity, bool batch = false,
            bool withConflicts = false, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(entity, nameof(entity));
            Guard.Against.NullOrWhiteSpace(entity.Id, nameof(entity.Id));
            TStateModel res;
            var doc = CreateCDoc(entity);
            var count = 0;
            return _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var oldDoc = await Db.FindAsync(doc.Id, withConflicts, cancellationToken);
                    if (oldDoc != null)
                        if (oldDoc.Rev != entity.Prev)
                            doc.Rev = oldDoc.Rev;
                    doc = await Db.AddOrUpdateAsync(doc, batch, cancellationToken);
                    res = doc.Data;
                    res.Prev = doc.Rev;
                    return res;
                }
                catch (Exception e)
                {
                    count++;
                    Logger?.Error($"{e.InnerAndOuter()}");
                    throw new Exception($"Retry/Backoff - {count * _backoff}ms", e);
                }
            });
        }




        
        


        protected RetryCouchStore(ICouchClient client, ILogger logger, AsyncRetryPolicy retryPolicy=null) : base(client, logger)
        {
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * _backoff));

        }

        protected RetryCouchStore(string dbName,
            string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc, AsyncRetryPolicy retryPolicy=null) : base(dbName,
            connectionString,
            couchSettingsFunc,
            flurlSettingsFunc)
        {
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * _backoff));

        }

        protected RetryCouchStore(ICouchClient clt,
            ILogger logger, AsyncRetryPolicy retryPolicy=null, ICouchDatabase<CDoc<TStateModel>> cachedDb = null) : base(clt,
            logger,
            cachedDb)
        {
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * _backoff));

        }
        
        
        
        
    }
}