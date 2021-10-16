using System;
using System.Text.Json;
using System.Threading.Tasks;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using CouchDB.Driver.Types;
using Flurl.Http.Configuration;
using M5x.Schemas;
using M5x.Store.Interfaces;
using Serilog;

namespace M5x.Store
{
    public class CouchStore<TReadModel> : ICouchStore<TReadModel> where TReadModel : IReadEntity
    {
        private readonly ICouchClient _clt;
        private readonly ILogger _logger;
        private ICouchDatabase<CDoc<TReadModel>> _cachedDb;

        public CouchStore(ICouchClient client, ILogger logger)
        {
            _clt = client;
            _logger = logger;
        }

        public CouchStore(string dbName, string connectionString, Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc)
        {
            _clt = new CouchClient(couchSettingsFunc);
        }

        private string DbName => GetDbName();
        public ICouchDatabase<CouchUser> UsersDb => GetUsersDb();

        public ICouchDatabase<CDoc<TReadModel>> Db => GetDb(DbName).Result;


        public async Task<TReadModel> AddOrUpdateAsync(TReadModel entity)
        {
            var doc = CreateCDoc(entity);
            try
            {
                var oldDoc = await Db.FindAsync(doc.Id);
                if (oldDoc != null) doc.Rev = oldDoc.Rev;
                doc = await Db.AddOrUpdateAsync(doc);
            }
            catch (Exception e)
            {
                _logger?.Fatal(JsonSerializer.Serialize(e.AsApiError()));
            }

            return doc.Data;
        }

        public async Task<TReadModel> GetByIdAsync(string id)
        {
            var doc = await Db.FindAsync(id);
            return doc == null ? default : doc.Data;
        }

        private string GetDbName()
        {
            var atts = (DbNameAttribute[]) typeof(TReadModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute [DbName] is missing on {typeof(TReadModel)}");
            return atts[0].DbName;
        }

        public async Task<ICouchDatabase<CDoc<TReadModel>>> GetDb(string dbName)
        {
            if (_cachedDb != null) return _cachedDb;
            _cachedDb = await _clt.GetOrCreateDatabaseAsync<CDoc<TReadModel>>(dbName);
            return _cachedDb;
            // ICouchDatabase<CDoc<TReadModel>> db = null;
            // var exists = await _clt.ExistsAsync(DbName);
            // if (!exists)
            // {
            //     db = await _clt.CreateDatabaseAsync<CDoc<TReadModel>>(DbName);
            // }
            // else
            // {
            //     db = _clt.GetDatabase<CDoc<TReadModel>>(dbName);
            // }
            // return db;
        }

        protected CDoc<TReadModel> CreateCDoc(TReadModel model)
        {
            return new()
            {
                Id = model.Id,
                Data = model
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