using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;
using Newtonsoft.Json;

namespace M5x.Consul.PreparedQuery
{
    public class PreparedQuery : IPreparedQueryEndpoint
    {
        private readonly ConsulClient.ConsulClient _client;

        internal PreparedQuery(ConsulClient.ConsulClient c)
        {
            _client = c;
        }

        public Task<WriteResult<string>> Create(PreparedQueryDefinition query,
            CancellationToken ct = default)
        {
            return Create(query, WriteOptions.Default, ct);
        }

        public async Task<WriteResult<string>> Create(PreparedQueryDefinition query, WriteOptions q,
            CancellationToken ct = default)
        {
            var res = await _client.Post<PreparedQueryDefinition, PreparedQueryCreationResult>("/v1/query", query, q)
                .Execute(ct).ConfigureAwait(false);
            return new WriteResult<string>(res, res.Response.Id);
        }

        public Task<WriteResult> Delete(string queryId, CancellationToken ct = default)
        {
            return Delete(queryId, WriteOptions.Default, ct);
        }

        public async Task<WriteResult> Delete(string queryId, WriteOptions q,
            CancellationToken ct = default)
        {
            var res = await _client.DeleteReturning<string>($"/v1/query/{queryId}", q).Execute(ct);
            return new WriteResult(res);
        }

        public Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIdOrName,
            CancellationToken ct = default)
        {
            return Execute(queryIdOrName, QueryOptions.Default, ct);
        }

        public Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIdOrName, QueryOptions q,
            CancellationToken ct = default)
        {
            return _client.Get<PreparedQueryExecuteResponse>($"/v1/query/{queryIdOrName}/execute", q)
                .Execute(ct);
        }

        public Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryId,
            CancellationToken ct = default)
        {
            return Get(queryId, QueryOptions.Default, ct);
        }

        public Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryId, QueryOptions q,
            CancellationToken ct = default)
        {
            return _client.Get<PreparedQueryDefinition[]>($"/v1/query/{queryId}", q).Execute(ct);
        }

        public Task<QueryResult<PreparedQueryDefinition[]>> List(CancellationToken ct = default)
        {
            return List(QueryOptions.Default, ct);
        }

        public Task<QueryResult<PreparedQueryDefinition[]>> List(QueryOptions q,
            CancellationToken ct = default)
        {
            return _client.Get<PreparedQueryDefinition[]>("/v1/query", q).Execute(ct);
        }

        public Task<WriteResult> Update(PreparedQueryDefinition query,
            CancellationToken ct = default)
        {
            return Update(query, WriteOptions.Default, ct);
        }

        public Task<WriteResult> Update(PreparedQueryDefinition query, WriteOptions q,
            CancellationToken ct = default)
        {
            return _client.Put($"/v1/query/{query.ID}", query, q).Execute(ct);
        }

        private class PreparedQueryCreationResult
        {
            [JsonProperty] internal string Id { get; set; }
        }
    }
}