using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Coordinate;

namespace M5x.Consul.Interfaces;

public interface ICoordinateEndpoint
{
    Task<QueryResult<CoordinateDatacenterMap[]>> Datacenters(CancellationToken ct = default);
    Task<QueryResult<CoordinateEntry[]>> Nodes(CancellationToken ct = default);
    Task<QueryResult<CoordinateEntry[]>> Nodes(QueryOptions q, CancellationToken ct = default);
}