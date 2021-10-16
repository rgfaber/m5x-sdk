using System;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.CEQS.Schema;

namespace M5x.CEQS.Infra.CouchDb
{
    public interface ICouchBuilder
    {
        ICouchStore<T> BuildStore<T>(string dbName, string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc = null,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc = null) 
            where T : IReadEntity;

        ICouchStore<TReadModel> BuildStore<TReadModel>() where TReadModel : IReadEntity;
    }
}