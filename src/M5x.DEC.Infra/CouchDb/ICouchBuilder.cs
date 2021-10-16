using System;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Schema;
using Polly.Retry;

namespace M5x.DEC.Infra.CouchDb
{
    public interface ICouchBuilder
    {
        ICouchStore<T> BuildStore<T>(string dbName, string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc = null,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc = null,
            AsyncRetryPolicy retryPolicy=null) where T : IReadEntity;

        ICouchStore<TReadModel> BuildStore<TReadModel>() where TReadModel : IReadEntity;
    }
}