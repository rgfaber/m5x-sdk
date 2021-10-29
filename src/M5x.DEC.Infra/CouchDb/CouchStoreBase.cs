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
using Serilog;

namespace M5x.DEC.Infra.CouchDb
{
    public abstract class CouchStoreBase<TStateModel, TId> : ICouchStore<TStateModel>
        where TStateModel : IStateEntity<TId>
        where TId : IIdentity
    {
        private readonly ICouchClient _clt;
        protected readonly ILogger Logger;
        private ICouchDatabase<CDoc<TStateModel>> _cachedDb;

        protected CouchStoreBase(ICouchClient client, ILogger logger)
        {
            _clt = client;
            Logger = logger;
        }

        protected CouchStoreBase(string dbName, string connectionString, Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc)
        {
            _clt = new CouchClient(couchSettingsFunc);
        }

        protected CouchStoreBase(ICouchClient clt, ILogger logger, ICouchDatabase<CDoc<TStateModel>> cachedDb = null)
        {
            _clt = clt;
            Logger = logger;
            _cachedDb = cachedDb;
        }

        private string DbName => GetDbName();
        protected ICouchDatabase<CouchUser> UsersDb => GetUsersDb();
        protected ICouchDatabase<CDoc<TStateModel>> Db => GetDb(DbName).Result;

        public abstract Task<TStateModel> AddOrUpdateAsync(TStateModel entity, bool batch = false,
            bool withConflicts = false,
            CancellationToken cancellationToken = default);

        public async Task<TStateModel> GetByIdAsync(string id)
        {
            Guard.Against.NullOrWhiteSpace(id, nameof(id));
            var doc = await Db.FindAsync(id);
            if (doc == null) return default;
            var res = doc.Data;
            res.Prev = doc.Rev;
            return res;
        }

        public async Task<TStateModel> GetByMeta(AggregateInfo meta)
        {
            Guard.Against.Null(meta, nameof(meta));
            Guard.Against.NullOrWhiteSpace(meta.Id, nameof(meta.Id));
            var res = await GetByIdAsync(meta.Id);
            if (res == null) return res;
            res.Meta ??= meta;
            return res;
        }


        public async Task<IEnumerable<TStateModel>> RetrieveRecent(int pageNumber, int pageSize)
        {
            Guard.Against.SmallerThanOne(pageNumber, nameof(pageNumber));
            Guard.Against.SmallerThanOne(pageSize, nameof(pageSize));
            await InitializeIndexesAsync();
            return Db.OrderByDescending(d => d.TimeStamp)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(x => x.Data)
                .ToArray();
        }


        public async Task<TStateModel> DeleteAsync(string id)
        {
            Guard.Against.NullOrWhiteSpace(id, nameof(id));
            var res = await GetByIdAsync(id);
            if (res == null) return default;
            var doc = CreateCDoc(res);
            await Db.RemoveAsync(doc);
            res.Prev = string.Empty;
            return res;
        }


        protected async Task InitializeIndexesAsync()
        {
            var indexes = await Db.GetIndexesAsync().ConfigureAwait(false);
            if (indexes.Exists(x => x.Name == "recent" && x.DesignDocument == "default")) return;
            await Db.CreateIndexAsync("recent",
                builder => { builder.IndexByDescending(d => d.TimeStamp); },
                new IndexOptions
                {
                    DesignDocument = "default",
                    Partitioned = false
                }).ConfigureAwait(false);
        }


        protected string GetDbName()
        {
            var atts = (DbNameAttribute[])typeof(TStateModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute [DbName] is missing on {typeof(TStateModel)}");
            return atts[0].DbName;
        }


        private async Task<ICouchDatabase<CDoc<TStateModel>>> GetDb(string dbName)
        {
            Guard.Against.NullOrWhiteSpace(dbName, nameof(dbName));
            if (_cachedDb != null) return _cachedDb;
            _cachedDb = await _clt.GetOrCreateDatabaseAsync<CDoc<TStateModel>>(dbName);
            return _cachedDb;
        }


        protected CDoc<TStateModel> CreateCDoc(TStateModel model)
        {
            Guard.Against.Null(model, nameof(model));
            Guard.Against.Null(model.Meta, nameof(model.Meta));
            Guard.Against.NullOrWhiteSpace(model.Meta.Id, nameof(model.Meta.Id));
            return new CDoc<TStateModel>
            {
                Id = model.Meta.Id,
                Rev = model.Prev,
                Data = model,
                TimeStamp = DateTime.UtcNow
            };
        }


        protected ICouchDatabase<CouchUser> GetUsersDb()
        {
            try
            {
                return _clt.GetUsersDatabase();
            }
            catch (Exception e)
            {
                Logger?.Error(e.InnerAndOuter());
                throw;
            }
        }
    }
}