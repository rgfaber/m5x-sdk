using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using M5x.Couch;
using M5x.DEC.Persistence;
using M5x.Schemas;
using M5x.Store.Interfaces;
using MyCouch;
using MyCouch.Net;
using MyCouch.Requests;
using Serilog;

namespace M5x.Store
{
    internal class XStore<TReadModel> : IStore where TReadModel : IReadEntity
    {
        private readonly IMyCouchStore _myCouch;

        protected string DbName;
        protected string Server;

        public XStore(string serverAddress, string dbName = null)
        {
            Server = serverAddress;
            DbName = dbName;
            _myCouch = new MyCouchStore(serverAddress, dbName);
        }

        public bool IsLocal { get; }

        /// POSTs or PUTs an entity to the database. 
        /// If ID is assigned in the Entity, 
        /// it will perform a PUT. If NO ID is assigned in the Entity, 
        /// it will perform a POST, and assign the DB GENERATED ID back to the entity. 
        /// If you have assigned BOTH ID and REV, a PUT that updates the current document will be performed.
        public async Task<T> StoreAsync<T>(T entity) where T : class
        {
            await OpenDb();
            return await _myCouch.StoreAsync(entity);
        }


        public async Task<Dictionary<string, XResponse>> OpenDb()
        {
            var res = new Dictionary<string, XResponse>();
            try
            {
                if (!CouchConfig.GetDbExists(DbName))
                {
                    var dbExists = await DbExists();
                    if (!dbExists)
                    {
                        Log.Information($"Creating Database {DbName}");
                        var pr = await _myCouch.Client.Database.PutAsync();
                        res.Add("db_create", pr.ToXResponse());
                        CouchConfig.SetDbExists(DbName);
                    }

                    if (CouchConfig.CanReplicate)
                    {
                        if (DbName.ToLower() == "_replicator") return res;
                        using var admin = new StoreBuilder<TReadModel>().BuildAdmin<TReadModel>();

                        var initResponse = await admin.Initialize();
                        res.Add("db_initialize", initResponse);

                        var repResponse = await admin.Replicate();
                        res.Add("db_replicate", repResponse);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, $"OpenDb for {DbName} => {e.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return res;
        }

        /// NOTE, NOTE, NOTE! An underlying lookup of latest known REVISION will be performed, then that revision will be used to to overwrite an existing document with entity .
        /// An initial HEAD will be performed to lookup the current revision. If you KNOW that the revision is allready assigned, use MyCouch.IMyCouchStore.StoreAsync1(0,System.Threading.CancellationToken) instead.
        public async Task<T> SetAsync<T>(T entity) where T : class
        {
            await OpenDb();
            return await _myCouch.SetAsync(entity);
        }

        public async Task<XBulkResponse> BulkAsync<T>(IEnumerable<T> documents, bool allOrNothing = false,
            bool newEdits = true)
            where T : class
        {
            var res = await OpenDb();

            var request = new BulkRequest
            {
                AllOrNothing = allOrNothing,
                NewEdits = newEdits
            };
            request.Include(documents
                .Select(_myCouch.Client.DocumentSerializer.Serialize)
                .ToArray());

            var bres = await _myCouch.Client.Documents.BulkAsync(request);
            return bres.ToXResponse();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await OpenDb();
            return await _myCouch.DeleteAsync(id);
        }

        public async Task<bool> DeleteAsync(string id, string rev)
        {
            await OpenDb();
            return await _myCouch.DeleteAsync(id, rev);
        }

        public async Task<bool> DeleteAsync<TEntity>(TEntity entity, bool lookupRev = false) where TEntity : class
        {
            await OpenDb();
            return await _myCouch.DeleteAsync(entity);
        }

        public async Task<bool> DocExists(string id, string rev = null)
        {
            await OpenDb();
            try
            {
                return await _myCouch.ExistsAsync(id, rev);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<string> GetByIdAsync(string id, string rev = null)
        {
            await OpenDb();
            return await _myCouch.GetByIdAsync(id, rev);
        }

        public async Task<TEntity> GetByIdAsync<TEntity>(string id, string rev = null) where TEntity : class
        {
            await OpenDb();
            return await _myCouch.GetByIdAsync<TEntity>(id, rev);
        }

        public async Task<IEnumerable<string>> GetByIdsAsync(params string[] ids)
        {
            await OpenDb();
            return await _myCouch.GetByIdsAsync(ids);
        }

        public async Task<IEnumerable<T>> GetByIdsAsync<T>(params string[] ids) where T : class
        {
            await OpenDb();
            return await _myCouch.GetByIdsAsync<T>(ids);
        }

        public async Task<XResponse> StoreAttachmentAsync(string docId, string docRev, string fileName,
            string contentType, byte[] content)
        {
            await OpenDb();
            PutAttachmentRequest request = null;
            var control = await _myCouch.GetHeaderAsync(docId);
            if (control != null) docRev = control.Rev;
            request = string.IsNullOrWhiteSpace(docRev)
                ? new PutAttachmentRequest(docId, fileName, contentType, content)
                : new PutAttachmentRequest(docId, docRev, fileName, contentType, content);
            var resp = await _myCouch.Client.Attachments.PutAsync(request);
            return resp.ToHeaderResponse();
        }

        public async Task<StreamResponse> GetAttachmentAsync(string docId, string docRev, string name)
        {
            await OpenDb();
            GetAttachmentRequest request = null;
            request = string.IsNullOrWhiteSpace(docRev)
                ? new GetAttachmentRequest(docId, name)
                : new GetAttachmentRequest(docId, docRev, name);

            var resp = await _myCouch.Client.Attachments.GetAsync(request);
            return resp.ToStreamResponse();
        }

        public async Task<string> GetStoreInfo()
        {
            var req = new ServerConnection(new ServerConnectionInfo(StoreConfig.LocalServer));
            var res = await req.SendAsync(new HttpRequest(HttpMethod.Get));
            return await res.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            _myCouch.Client.Connection.Dispose();
            _myCouch.Dispose();
        }

        public int AddDesignDocument(string viewName, string designDoc, string code)
        {
            var clt = new CouchServer(CouchConfig.LocalHost,
                CouchConfig.LocalPort,
                CouchConfig.LocalUser,
                CouchConfig.LocalPwd);
            var docs = clt.GetDatabase(DbName);
            if (docs.HasDocument($"_design/{designDoc}"))
                return 0;
            var dDoc = docs.NewDesignDocument(designDoc);
            var def = dDoc.AddView(viewName, code);
            dDoc.Synch();
            var q = def.Query();
            return q.Result?
                .TotalCount() ?? 0;
        }

        public int AddDesignDocument(string viewName, string designDoc, string mapCode, string reduceCode)
        {
            var clt = new CouchServer(CouchConfig.LocalHost,
                CouchConfig.LocalPort,
                CouchConfig.LocalUser,
                CouchConfig.LocalPwd);

            var docs = clt.GetDatabase(DbName);
            if (docs.HasDocument($"_design/{designDoc}"))
                return 0;

            var dDoc = docs.NewDesignDocument(designDoc);
            var def = dDoc.AddView(viewName, mapCode, reduceCode);
            dDoc.Synch();
            var q = def.Query();
            return q.Result?
                .TotalCount() ?? 0;
        }

        public async Task<QueryViewDoc<T>> QueryView<T>(string designDoc, string viewName) where T : class, new()
        {
            var docs = new List<T>();
            var keys = new List<string>();
            var qvd = new QueryViewDoc<T>();
            await OpenDb();
            var req = CreateQueryViewRequest(designDoc, viewName);
            var resp = await _myCouch.Client.Views.QueryAsync(req);
            if (resp.IsEmpty) return null;
            foreach (var respRow in resp.Rows)
            {
                docs.Add(_myCouch.Client.DocumentSerializer.Deserialize<T>(respRow.IncludedDoc));
                keys.Add(respRow.Value);
            }

            qvd.Docs = docs;
            qvd.Keys = keys;
            return qvd;
        }


        public async Task<ViewResponse<TKey, TValue>> QueryView<TKey, TValue>(string designDoc, string viewName,
            bool includeDocs = false)
        {
            await OpenDb();

            var req = new QueryViewRequest(designDoc, viewName) {IncludeDocs = includeDocs};
            var resp = await _myCouch.Client.Views.QueryRawAsync(req);
            if (resp.IsEmpty)
                return null;

            try
            {
                return _myCouch.Client.DocumentSerializer.Deserialize<ViewResponse<TKey, TValue>>(resp.Content);
            }
            catch (Exception ex)
            {
                return new ViewResponse<TKey, TValue> {Exception = ex};
            }
        }

        public async Task<bool> DbExists()
        {
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            try
            {
                var res = await client.Databases.HeadAsync(DbName);
                return res.IsSuccess;
            }
            catch (Exception e)
            {
                Log.Warning($"Database {DbName} does not exist.");
                return false;
            }
        }

        public async Task BulkDelete()
        {
            await Task.Run(() =>
            {
                var clt = new CouchServer(CouchConfig.LocalHost,
                    CouchConfig.LocalPort,
                    CouchConfig.LocalUser,
                    CouchConfig.LocalPwd);
                var docs = clt.GetDatabase(DbName);
                var d = docs.GetAllDocuments();
                docs.DeleteDocuments(d);
            });
        }

        private QueryViewRequest CreateQueryViewRequest(string designDoc, string viewName)
        {
            var req = new QueryViewRequest(designDoc, viewName) {IncludeDocs = true};
            return req;
        }
    }
}