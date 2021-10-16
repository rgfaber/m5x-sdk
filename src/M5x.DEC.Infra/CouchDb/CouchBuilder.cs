using System;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Schema;
using Polly.Retry;

namespace M5x.DEC.Infra.CouchDb
{
    public class CouchBuilder : ICouchBuilder
    {
        public abstract ICouchStore<TReadModel> BuildStore<TReadModel>(string dbName,
            string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc = null,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc = null,
            AsyncRetryPolicy retryPolicy = null) where TReadModel : IReadEntity;

        public ICouchStore<TReadModel> BuildStore<TReadModel>() where TReadModel : IReadEntity
        {
            return BuildStore<TReadModel>(GetDbName<TReadModel>());
        }

        private string GetDbName<TReadModel>()
        {
            var atts = (DbNameAttribute[]) typeof(TReadModel).GetCustomAttributes(typeof(DbNameAttribute), false);
            if (atts.Length == 0) throw new Exception($"{typeof(TReadModel)} does not have a DbName attribute!");
            return atts[0].DbName;
        }


        public ICouchStore<T> BuildStore<T>(string dbName) where T : IReadEntity
        {
            return new CouchStore<T>(dbName, CouchConfig.LocalSource, settings =>
            {
                settings.UseBasicAuthentication(CouchConfig.LocalUser, CouchConfig.LocalPwd);
                settings.EnsureDatabaseExists();
            }, settings => { }, null);
        }
    }
}