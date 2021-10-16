using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using M5x.Store;
using M5x.Store.Interfaces;

namespace M5x.LiteDb
{
    public class LiteStore : IStore
    {
        private readonly LiteDatabase _db;

        public LiteStore(ConnectionString connection)
        {
            _db = new LiteDatabase(connection);
        }

        public async Task<string> GetStoreInfo()
        {
            throw new NotImplementedException();
        }


        public async Task<T> StoreAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<T> SetAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(string id, string rev)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync<TEntity>(TEntity entity, bool lookupRev = false) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DocExists(string id, string rev = null)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetByIdAsync(string id, string rev = null)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(string id, string rev = null) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetByIdsAsync(params string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetByIdsAsync<T>(params string[] ids) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<XResponse> StoreAttachmentAsync(string docId, string docRev, string filename,
            string contentType, byte[] content)
        {
            throw new NotImplementedException();
        }

        public async Task<StreamResponse> GetAttachmentAsync(string docId, string docRev, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, XResponse>> OpenDb()
        {
            throw new NotImplementedException();
        }

        public async Task<XBulkResponse> BulkAsync<T>(IEnumerable<T> entities, bool allOrNothing = false,
            bool newEdits = true) where T : class
        {
            throw new NotImplementedException();
        }

        public int AddDesignDocument(string viewName, string designDoc, string code)
        {
            throw new NotImplementedException();
        }

        public int AddDesignDocument(string viewName, string designDoc, string mapCode, string reduceCode)
        {
            throw new NotImplementedException();
        }

        public async Task<QueryViewDoc<T>> QueryView<T>(string designDoc, string viewName) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<ViewResponse<TKey, TValue>> QueryView<TKey, TValue>(string designDoc, string viewName,
            bool includeDocs = false)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}