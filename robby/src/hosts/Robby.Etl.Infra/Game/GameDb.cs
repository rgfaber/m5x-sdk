using System;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Infra.CouchDb;
using Serilog;

namespace Robby.Etl.Infra.Game
{
    public interface IGameDb : ICouchStore<Schema.Game>
    {
    }

    internal class GameDb : CouchStore<Schema.Game>, IGameDb
    {
        public GameDb(ICouchClient client, ILogger logger) : base(client, logger)
        {
        }

        public GameDb(string dbName,
            string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc) : base(dbName,
            connectionString,
            couchSettingsFunc,
            flurlSettingsFunc)
        {
        }
    }
}