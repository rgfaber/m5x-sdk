using System;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Infra.CouchDb;
using Robby.Schema;
using Serilog;

namespace Robby.Infra
{
    
    internal class RoboSimStore: CouchStore<RoboSim>, IRoboSimStore
    {
        public RoboSimStore(ICouchClient client, ILogger logger) : base(client, logger)
        {
        }

        public RoboSimStore(string dbName,
            string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc) : base(dbName,
            connectionString,
            couchSettingsFunc,
            flurlSettingsFunc)
        {
        }
    }
    
    
    public interface IRoboSimStore : ICouchStore<RoboSim>
    {
    }
}