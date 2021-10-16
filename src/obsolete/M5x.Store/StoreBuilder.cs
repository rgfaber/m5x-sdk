using System;
using M5x.Schemas;
using M5x.Store.Interfaces;


namespace M5x.Store
{
    public class StoreBuilder<TReadModel> : IStoreBuilder<TReadModel> where TReadModel : IReadEntity
    {
        public IAdminStore<TReadModel> BuildAdmin(string dbName)
        {
            dbName = dbName.ToValidDbName();
            return new XAdminStore<TReadModel>(StoreConfig.LocalServer, dbName);
        }


        public IAdminStore<TReadModel> BuildAdmin<TReadModel>() where TReadModel : IReadEntity
        {
            var dbName = GetDbName<TReadModel>().ToValidDbName();
            return new XAdminStore<TReadModel>(StoreConfig.LocalServer, dbName);
        }


        public IStore Build(string dbName)
        {
            dbName = dbName.ToValidDbName();
            return new XStore<TReadModel>(StoreConfig.LocalServer, dbName);
        }


        public IStore Build(string remoteAddress, string dbName)
        {
            dbName = dbName.ToValidDbName();
            return new XStore<TReadModel>(remoteAddress, dbName);
        }


        public IAdminStore<TReadModel> BuildAdmin(string remoteAddress, string dbName)
        {
            dbName = dbName.ToValidDbName();
            return new XAdminStore<TReadModel>(remoteAddress, dbName);
        }

        private string GetDbName<TReadModel>()
        {
            var atts =
                (DbNameAttribute[]) typeof(TReadModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length <= 0) throw new Exception($"No [DbName] attribute specified on {typeof(TReadModel)}");
            var att = atts[0];
            return att.DbName;
        }
    }
}