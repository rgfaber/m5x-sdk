using System;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.Consul.Interfaces;

public interface IUriPool
{
    CancellationToken RefreshToken { get; }
    Task<Uri> GetUri(string key);
}