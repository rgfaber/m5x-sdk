using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Coordinate.Coordinate> _coordinate;

    /// <summary>
    ///     Session returns a handle to the session endpoints
    /// </summary>
    public ICoordinateEndpoint Coordinate => _coordinate.Value;
}