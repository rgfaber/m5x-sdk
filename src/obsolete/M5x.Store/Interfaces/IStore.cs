using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5x.Store.Interfaces
{
    public interface IStore : IPingStore, IDisposable
    {
        Task<T> StoreAsync<T>(T entity) where T : class;
        Task<T> SetAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteAsync(string id, string rev);
        Task<bool> DeleteAsync<TEntity>(TEntity entity, bool lookupRev = false) where TEntity : class;
        Task<bool> DocExists(string id, string rev = null);
        Task<string> GetByIdAsync(string id, string rev = null);
        Task<TEntity> GetByIdAsync<TEntity>(string id, string rev = null) where TEntity : class;
        Task<IEnumerable<string>> GetByIdsAsync(params string[] ids);
        Task<IEnumerable<T>> GetByIdsAsync<T>(params string[] ids) where T : class;

        Task<XResponse> StoreAttachmentAsync(string docId, string docRev, string filename, string contentType,
            byte[] content);

        Task<StreamResponse> GetAttachmentAsync(string docId, string docRev, string name);

        Task<Dictionary<string, XResponse>> OpenDb();

        Task<XBulkResponse> BulkAsync<T>(IEnumerable<T> entities, bool allOrNothing = false, bool newEdits = true)
            where T : class;

        int AddDesignDocument(string viewName, string designDoc, string code);
        int AddDesignDocument(string viewName, string designDoc, string mapCode, string reduceCode);
        Task<QueryViewDoc<T>> QueryView<T>(string designDoc, string viewName) where T : class, new();

        Task<ViewResponse<TKey, TValue>> QueryView<TKey, TValue>(string designDoc, string viewName,
            bool includeDocs = false);
    }
}