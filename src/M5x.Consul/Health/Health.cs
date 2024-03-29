﻿// -----------------------------------------------------------------------
//  <copyright file="Health.cs" company="PlayFab Inc">
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

namespace M5x.Consul.Health;

/// <summary>
///     Health can be used to query the Health endpoints
/// </summary>
public class Health : IHealthEndpoint
{
    private readonly ConsulClient.ConsulClient _client;

    internal Health(ConsulClient.ConsulClient c)
    {
        _client = c;
    }

    /// <summary>
    ///     Checks is used to return the checks associated with a service
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <returns>
    ///     A query result containing the health checks matching the provided service ID, or a query result with a null
    ///     response if no service matched the provided ID
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> Checks(string service,
        CancellationToken ct = default)
    {
        return Checks(service, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Checks is used to return the checks associated with a service
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>
    ///     A query result containing the health checks matching the provided service ID, or a query result with a null
    ///     response if no service matched the provided ID
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> Checks(string service, QueryOptions q,
        CancellationToken ct = default)
    {
        return _client.Get<HealthCheck[]>($"/v1/health/checks/{service}", q).Execute(ct);
    }

    /// <summary>
    ///     Node is used to query for checks belonging to a given node
    /// </summary>
    /// <param name="node">The node name</param>
    /// <returns>
    ///     A query result containing the health checks matching the provided node ID, or a query result with a null
    ///     response if no node matched the provided ID
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> Node(string node, CancellationToken ct = default)
    {
        return Node(node, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Node is used to query for checks belonging to a given node
    /// </summary>
    /// <param name="node">The node name</param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>
    ///     A query result containing the health checks matching the provided node ID, or a query result with a null
    ///     response if no node matched the provided ID
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> Node(string node, QueryOptions q,
        CancellationToken ct = default)
    {
        return _client.Get<HealthCheck[]>($"/v1/health/node/{node}", q).Execute(ct);
    }

    /// <summary>
    ///     Service is used to query health information along with service info for a given service. It can optionally do
    ///     server-side filtering on a tag or nodes with passing health checks only.
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <returns>
    ///     A query result containing the service members matching the provided service ID, or a query result with a null
    ///     response if no service members matched the filters provided
    /// </returns>
    public Task<QueryResult<ServiceEntry[]>> Service(string service,
        CancellationToken ct = default)
    {
        return Service(service, string.Empty, false, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Service is used to query health information along with service info for a given service. It can optionally do
    ///     server-side filtering on a tag or nodes with passing health checks only.
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <param name="tag">The service member tag</param>
    /// <returns>
    ///     A query result containing the service members matching the provided service ID and tag, or a query result with
    ///     a null response if no service members matched the filters provided
    /// </returns>
    public Task<QueryResult<ServiceEntry[]>> Service(string service, string tag,
        CancellationToken ct = default)
    {
        return Service(service, tag, false, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Service is used to query health information along with service info for a given service. It can optionally do
    ///     server-side filtering on a tag or nodes with passing health checks only.
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <param name="tag">The service member tag</param>
    /// <param name="passingOnly">Only return if the health check is in the Passing state</param>
    /// <returns>
    ///     A query result containing the service members matching the provided service ID, tag, and health status, or a
    ///     query result with a null response if no service members matched the filters provided
    /// </returns>
    public Task<QueryResult<ServiceEntry[]>> Service(string service, string tag, bool passingOnly,
        CancellationToken ct = default)
    {
        return Service(service, tag, passingOnly, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     Service is used to query health information along with service info for a given service. It can optionally do
    ///     server-side filtering on a tag or nodes with passing health checks only.
    /// </summary>
    /// <param name="service">The service ID</param>
    /// <param name="tag">The service member tag</param>
    /// <param name="passingOnly">Only return if the health check is in the Passing state</param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>
    ///     A query result containing the service members matching the provided service ID, tag, and health status, or a
    ///     query result with a null response if no service members matched the filters provided
    /// </returns>
    public Task<QueryResult<ServiceEntry[]>> Service(string service, string tag, bool passingOnly, QueryOptions q,
        CancellationToken ct = default)
    {
        var req = _client.Get<ServiceEntry[]>($"/v1/health/service/{service}", q);
        if (!string.IsNullOrEmpty(tag)) req.Params["tag"] = tag;
        if (passingOnly) req.Params["passing"] = "1";
        return req.Execute(ct);
    }

    /// <summary>
    ///     State is used to retrieve all the checks in a given state. The wildcard "any" state can also be used for all
    ///     checks.
    /// </summary>
    /// <param name="status">The health status to filter for</param>
    /// <returns>
    ///     A query result containing a list of health checks in the specified state, or a query result with a null
    ///     response if no health checks matched the provided state
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> State(HealthStatus status,
        CancellationToken ct = default)
    {
        return State(status, QueryOptions.Default, ct);
    }

    /// <summary>
    ///     // State is used to retrieve all the checks in a given state. The wildcard "any" state can also be used for all
    ///     checks.
    /// </summary>
    /// <param name="status">The health status to filter for</param>
    /// <param name="q">Customized query options</param>
    /// <param name="ct">
    ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
    ///     request is cancelled before completing
    /// </param>
    /// <returns>
    ///     A query result containing a list of health checks in the specified state, or a query result with a null
    ///     response if no health checks matched the provided state
    /// </returns>
    public Task<QueryResult<HealthCheck[]>> State(HealthStatus status, QueryOptions q,
        CancellationToken ct = default)
    {
        return _client.Get<HealthCheck[]>($"/v1/health/state/{status.Status}", q).Execute(ct);
    }
}