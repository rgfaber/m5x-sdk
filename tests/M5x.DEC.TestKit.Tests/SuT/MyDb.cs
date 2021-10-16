using System;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Infra.CouchDb;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    
    public interface IMyDb: ICouchStore<MyReadModel> {}
    
    public class MyDb: CouchStore<MyReadModel,MyID>, IMyDb
    {
        public MyDb(ICouchClient client, ILogger logger) : base(client, logger)
        {
        }

        public MyDb(string dbName,
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