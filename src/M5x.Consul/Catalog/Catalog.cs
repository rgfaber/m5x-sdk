﻿// -----------------------------------------------------------------------
//  <copyright file="Catalog.cs" company="PlayFab Inc">
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;

namespace M5x.Consul.Catalog
{
    /// <summary>
    ///     Catalog can be used to query the Catalog endpoints
    /// </summary>
    public class Catalog : ICatalogEndpoint
    {
        private readonly ConsulClient.ConsulClient _client;

        internal Catalog(ConsulClient.ConsulClient c)
        {
            _client = c;
        }

        /// <summary>
        ///     Register a new catalog item
        /// </summary>
        /// <param name="reg">A catalog registration</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Register(CatalogRegistration reg, CancellationToken ct = default)
        {
            return Register(reg, WriteOptions.Default, ct);
        }

        /// <summary>
        ///     Register a new catalog item
        /// </summary>
        /// <param name="reg">A catalog registration</param>
        /// <param name="q">Customized write options</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Register(CatalogRegistration reg, WriteOptions q,
            CancellationToken ct = default)
        {
            return _client.Put("/v1/catalog/register", reg, q).Execute(ct);
        }

        /// <summary>
        ///     Deregister an existing catalog item
        /// </summary>
        /// <param name="reg">A catalog deregistration</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Deregister(CatalogDeregistration reg,
            CancellationToken ct = default)
        {
            return Deregister(reg, WriteOptions.Default, ct);
        }

        /// <summary>
        ///     Deregister an existing catalog item
        /// </summary>
        /// <param name="reg">A catalog deregistration</param>
        /// <param name="q">Customized write options</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Deregister(CatalogDeregistration reg, WriteOptions q,
            CancellationToken ct = default)
        {
            return _client.Put("/v1/catalog/deregister", reg, q).Execute(ct);
        }

        /// <summary>
        ///     Datacenters is used to query for all the known datacenters
        /// </summary>
        /// <returns>A list of datacenter names</returns>
        public Task<QueryResult<string[]>> Datacenters(CancellationToken ct = default)
        {
            return _client.Get<string[]>("/v1/catalog/datacenters").Execute(ct);
        }

        /// <summary>
        ///     Nodes is used to query all the known nodes
        /// </summary>
        /// <returns>A list of all nodes</returns>
        public Task<QueryResult<Node[]>> Nodes(CancellationToken ct = default)
        {
            return Nodes(QueryOptions.Default, ct);
        }

        /// <summary>
        ///     Nodes is used to query all the known nodes
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>A list of all nodes</returns>
        public Task<QueryResult<Node[]>> Nodes(QueryOptions q, CancellationToken ct = default)
        {
            return _client.Get<Node[]>("/v1/catalog/nodes", q).Execute(ct);
        }

        /// <summary>
        ///     Services is used to query for all known services
        /// </summary>
        /// <returns>A list of all services</returns>
        public Task<QueryResult<Dictionary<string, string[]>>> Services(
            CancellationToken ct = default)
        {
            return Services(QueryOptions.Default, ct);
        }

        /// <summary>
        ///     Services is used to query for all known services
        /// </summary>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>A list of all services</returns>
        public Task<QueryResult<Dictionary<string, string[]>>> Services(QueryOptions q,
            CancellationToken ct = default)
        {
            return _client.Get<Dictionary<string, string[]>>("/v1/catalog/services", q).Execute(ct);
        }

        /// <summary>
        ///     Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service,
            CancellationToken ct = default)
        {
            return Service(service, string.Empty, QueryOptions.Default, ct);
        }

        /// <summary>
        ///     Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="tag">A tag to filter on</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service, string tag,
            CancellationToken ct = default)
        {
            return Service(service, tag, QueryOptions.Default, ct);
        }

        /// <summary>
        ///     Service is used to query catalog entries for a given service
        /// </summary>
        /// <param name="service">The service ID</param>
        /// <param name="tag">A tag to filter on</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>A list of service instances</returns>
        public Task<QueryResult<CatalogService[]>> Service(string service, string tag, QueryOptions q,
            CancellationToken ct)
        {
            var req = _client.Get<CatalogService[]>($"/v1/catalog/service/{service}", q);
            if (!string.IsNullOrEmpty(tag)) req.Params["tag"] = tag;
            return req.Execute(ct);
        }

        /// <summary>
        ///     Node is used to query for service information about a single node
        /// </summary>
        /// <param name="node">The node name</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>The node information including a list of services</returns>
        public Task<QueryResult<CatalogNode>> Node(string node, CancellationToken ct = default)
        {
            return Node(node, QueryOptions.Default, ct);
        }

        /// <summary>
        ///     Node is used to query for service information about a single node
        /// </summary>
        /// <param name="node">The node name</param>
        /// <param name="q">Customized query options</param>
        /// <param name="ct">
        ///     Cancellation token for long poll request. If set, OperationCanceledException will be thrown if the
        ///     request is cancelled before completing
        /// </param>
        /// <returns>The node information including a list of services</returns>
        public Task<QueryResult<CatalogNode>> Node(string node, QueryOptions q,
            CancellationToken ct = default)
        {
            return _client.Get<CatalogNode>($"/v1/catalog/node/{node}", q).Execute(ct);
        }
    }
}