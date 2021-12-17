using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.PreparedQuery;

namespace M5x.Consul.Interfaces;

public interface IPreparedQueryEndpoint
{
    Task<WriteResult<string>> Create(PreparedQueryDefinition query,
        CancellationToken ct = default);

    Task<WriteResult<string>> Create(PreparedQueryDefinition query, WriteOptions q,
        CancellationToken ct = default);

    Task<WriteResult> Update(PreparedQueryDefinition query, CancellationToken ct = default);

    Task<WriteResult> Update(PreparedQueryDefinition query, WriteOptions q,
        CancellationToken ct = default);

    Task<QueryResult<PreparedQueryDefinition[]>> List(CancellationToken ct = default);

    Task<QueryResult<PreparedQueryDefinition[]>> List(QueryOptions q,
        CancellationToken ct = default);

    Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryId,
        CancellationToken ct = default);

    Task<QueryResult<PreparedQueryDefinition[]>> Get(string queryId, QueryOptions q,
        CancellationToken ct = default);

    Task<WriteResult> Delete(string queryId, CancellationToken ct = default);
    Task<WriteResult> Delete(string queryId, WriteOptions q, CancellationToken ct = default);

    Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIdOrName,
        CancellationToken ct = default);

    Task<QueryResult<PreparedQueryExecuteResponse>> Execute(string queryIdOrName, QueryOptions q,
        CancellationToken ct = default);
}