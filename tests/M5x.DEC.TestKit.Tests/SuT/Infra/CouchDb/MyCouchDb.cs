using System;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Infra.CouchDb;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb
{
    
    public interface IMyCouchDb: ICouchStore<MyReadModel> {}
    
    public class MyCouchDb: CouchStore<MyReadModel,MyID>, IMyCouchDb
    {
        public MyCouchDb(ICouchClient client, ILogger logger) : base(client, logger)
        {
        }

        public MyCouchDb(string dbName,
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