using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;

namespace M5x.Consul.Interfaces;

public interface ISnapshotEndpoint
{
    Task<QueryResult<Stream>> Save(CancellationToken ct = default);
    Task<QueryResult<Stream>> Save(QueryOptions q, CancellationToken ct = default);
    Task<WriteResult> Restore(Stream s, CancellationToken ct = default);
    Task<WriteResult> Restore(Stream s, WriteOptions q, CancellationToken ct = default);
}