// -----------------------------------------------------------------------
//  <copyright file="ClientTest.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;

namespace M5x.Consul.Coordinate;
// May want to rework this as Dictionary<string,List<CoordinateEntry>>

public class Coordinate : ICoordinateEndpoint
{
    private readonly ConsulClient.ConsulClient _client;

    internal Coordinate(ConsulClient.ConsulClient c)
    {
        _client = c;
    }

    /// <summary>
    ///     Datacenters is used to return the coordinates of all the servers in the WAN pool.
    /// </summary>
    /// <returns>
    ///     A query result containing a map of datacenters, each with a list of coordinates of all the servers in the WAN
    ///     pool
    /// </returns>
    public Task<QueryResult<CoordinateDatacenterMap[]>> Datacenters(
        CancellationToken ct = default)
    {
        return _client.Get<CoordinateDatacenterMap[]>("/v1/coordinate/datacenters").Execute(ct);
    }

    /// <summary>
    ///     Nodes is used to return the coordinates of all the nodes in the LAN pool.
    /// </summary>
    /// <returns>A query result containing coordinates of all the nodes in the LAN pool</returns>
    public Task<QueryResult<CoordinateEntry[]>> Nodes(CancellationToken ct = default)
    {
        return Nodes(QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Nodes is used to return the coordinates of all the nodes in the LAN pool.
    /// </summary>
    /// <param name="q">Customized query options</param>
    /// <returns>A query result containing coordinates of all the nodes in the LAN pool</returns>
    public Task<QueryResult<CoordinateEntry[]>> Nodes(QueryOptions q,
        CancellationToken ct = default)
    {
        return _client.Get<CoordinateEntry[]>("/v1/coordinate/nodes", q).Execute(ct);
    }
}