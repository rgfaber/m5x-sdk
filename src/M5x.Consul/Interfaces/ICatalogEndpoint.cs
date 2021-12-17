using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Catalog;
using M5x.Consul.Client;

namespace M5x.Consul.Interfaces;

public interface ICatalogEndpoint
{
    Task<QueryResult<string[]>> Datacenters(CancellationToken ct = default);
    Task<WriteResult> Deregister(CatalogDeregistration reg, CancellationToken ct = default);

    Task<WriteResult> Deregister(CatalogDeregistration reg, WriteOptions q,
        CancellationToken ct = default);

    Task<QueryResult<CatalogNode>> Node(string node, CancellationToken ct = default);

    Task<QueryResult<CatalogNode>> Node(string node, QueryOptions q,
        CancellationToken ct = default);

    Task<QueryResult<Node[]>> Nodes(CancellationToken ct = default);
    Task<QueryResult<Node[]>> Nodes(QueryOptions q, CancellationToken ct = default);
    Task<WriteResult> Register(CatalogRegistration reg, CancellationToken ct = default);

    Task<WriteResult> Register(CatalogRegistration reg, WriteOptions q,
        CancellationToken ct = default);

    Task<QueryResult<CatalogService[]>> Service(string service, CancellationToken ct = default);

    Task<QueryResult<CatalogService[]>> Service(string service, string tag,
        CancellationToken ct = default);

    Task<QueryResult<CatalogService[]>> Service(string service, string tag, QueryOptions q,
        CancellationToken ct = default);

    Task<QueryResult<Dictionary<string, string[]>>> Services(CancellationToken ct = default);

    Task<QueryResult<Dictionary<string, string[]>>> Services(QueryOptions q,
        CancellationToken ct = default);
}