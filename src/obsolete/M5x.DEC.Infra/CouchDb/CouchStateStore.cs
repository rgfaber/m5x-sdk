using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CouchDB.Driver;
using CouchDB.Driver.Indexes;
using CouchDB.Driver.Options;
using CouchDB.Driver.Types;
using Flurl.Http.Configuration;
using M5x.Schemas;
using Serilog;

namespace M5x.DEC.Infra.CouchDb
{
    public class CouchStateStore<TStateModel,TId> : ICouchStore<TStateModel>
         where TId: IAggregateID
        where TStateModel : IStateEntity<TId>
    {
        private readonly ICouchClient _clt;
        private readonly ILogger _logger;
        private ICouchDatabase<CDoc<TStateModel>> _cachedDb;

        public CouchStateStore(ICouchClient client, ILogger logger)
        {
            _clt = client;
            _logger = logger;
        }

        public CouchStateStore(string dbName, string connectionString, Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc)
        {
            _clt = new CouchClient(couchSettingsFunc);
        }

        private string DbName => GetDbName();
        public ICouchDatabase<CouchUser> UsersDb => GetUsersDb();

        public ICouchDatabase<CDoc<TStateModel>> Db => GetDb(DbName).Result;


        public async Task<TStateModel> AddOrUpdateAsync(TStateModel entity, bool batch=false, bool withConflicts=false, CancellationToken cancellationToken=default(CancellationToken))
        {
            TStateModel res;
            var doc = CreateCDoc(entity);
            try
            {
                var oldDoc = await Db.FindAsync(doc.Id, withConflicts, cancellationToken);
                if (oldDoc != null)
                {
                    if(oldDoc.Rev!=entity.Prev)
                    {
                        doc.Rev = oldDoc.Rev;
                    }
                }
                doc = await Db.AddOrUpdateAsync(doc,batch,cancellationToken);
                res = doc.Data;
                res.Prev = doc.Rev;
            }
            catch (Exception e)
            {
                _logger?.Error(JsonSerializer.Serialize(e.AsApiError()));
                throw;
            }
            return res;
        }
        

        public async Task<TStateModel> GetByIdAsync(string id)
        {
            var doc = await Db.FindAsync(id);
            return doc == null ? default : doc.Data;
        }

        public async Task<IEnumerable<TStateModel>> RetrieveRecent(int pageNumber, int pageSize)
        {
            var index = await Db.CreateIndexAsync( "recent", 
                builder =>
                {
                    builder.IndexByDescending(d => d.TimeStamp);
                }, 
                new IndexOptions()
                {
                    DesignDocument = "default",
                    Partitioned = false
                });
            return Db.OrderByDescending(d => d.TimeStamp)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(x => x.Data)
                .ToArray();
        }

        private string GetDbName()
        {
            var atts = (DbNameAttribute[]) typeof(TStateModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute [DbName] is missing on {typeof(TStateModel)}");
            return atts[0].DbName;
        }

        public async Task<ICouchDatabase<CDoc<TStateModel>>> GetDb(string dbName)
        {
            if (_cachedDb != null) return _cachedDb;
            _cachedDb = await _clt.GetOrCreateDatabaseAsync<CDoc<TStateModel>>(dbName);
            return _cachedDb;
        }

        protected CDoc<TStateModel> CreateCDoc(TStateModel model)
        {
            return new()
            {
                Id = model.AggregateId.Value,
                Rev = model.Prev,
                Data = model,
                TimeStamp = DateTime.UtcNow
            };
        }


        private ICouchDatabase<CouchUser> GetUsersDb()
        {
            try
            {
                return _clt.GetUsersDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}