// -----------------------------------------------------------------------
//  <copyright file="Agent.cs" company="PlayFab Inc">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Interfaces;
using M5x.Consul.Utilities;

namespace M5x.Consul.Agent
{
    /// <summary>
    ///     Agent can be used to query the Agent endpoints
    /// </summary>
    public class Agent : IAgentEndpoint
    {
        private readonly ConsulClient.ConsulClient _client;
        private readonly AsyncLock _nodeNameLock;
        private string _nodeName;

        internal Agent(ConsulClient.ConsulClient c)
        {
            _client = c;
            _nodeNameLock = new AsyncLock();
        }

        /// <summary>
        ///     Self is used to query the agent we are speaking to for information about itself
        /// </summary>
        /// <returns>A somewhat dynamic object representing the various data elements in Self</returns>
        public Task<QueryResult<Dictionary<string, Dictionary<string, dynamic>>>> Self(
            CancellationToken ct = default)
        {
            return _client.Get<Dictionary<string, Dictionary<string, dynamic>>>("/v1/agent/self").Execute(ct);
        }

        /// <summary>
        ///     NodeName is used to get the node name of the agent
        /// </summary>
        [Obsolete("This property will be removed in 0.8.0. Replace uses of it with a call to 'await GetNodeName()'")]
        public string NodeName => GetNodeName().ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        ///     GetNodeName is used to get the node name of the agent. The value is cached per instance of ConsulClient after the
        ///     first use.
        /// </summary>
        public async Task<string> GetNodeName(CancellationToken ct = default)
        {
            if (_nodeName == null)
                using (await _nodeNameLock.LockAsync().ConfigureAwait(false))
                {
                    if (_nodeName == null) _nodeName = (await Self(ct)).Response["Config"]["NodeName"];
                }

            return _nodeName;
        }

        /// <summary>
        ///     Checks returns the locally registered checks
        /// </summary>
        /// <returns>A map of the registered check names and check data</returns>
        public Task<QueryResult<Dictionary<string, AgentCheck>>> Checks(
            CancellationToken ct = default)
        {
            return _client.Get<Dictionary<string, AgentCheck>>("/v1/agent/checks").Execute(ct);
        }

        /// <summary>
        ///     Services returns the locally registered services
        /// </summary>
        /// <returns>A map of the registered services and service data</returns>
        public Task<QueryResult<Dictionary<string, AgentService>>> Services(
            CancellationToken ct = default)
        {
            return _client.Get<Dictionary<string, AgentService>>("/v1/agent/services").Execute(ct);
        }

        /// <summary>
        ///     Members returns the known gossip members. The WAN flag can be used to query a server for WAN members.
        /// </summary>
        /// <returns>An array of gossip peers</returns>
        public Task<QueryResult<AgentMember[]>> Members(bool wan, CancellationToken ct = default)
        {
            var req = _client.Get<AgentMember[]>("/v1/agent/members");
            if (wan) req.Params["wan"] = "1";
            return req.Execute(ct);
        }

        /// <summary>
        ///     ServiceRegister is used to register a new service with the local agent
        /// </summary>
        /// <param name="service">A service registration object</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> ServiceRegister(AgentServiceRegistration service,
            CancellationToken ct = default)
        {
            return _client.Put("/v1/agent/service/register", service).Execute(ct);
        }

        /// <summary>
        ///     ServiceRegister is used to register a new service with the local agent
        /// </summary>
        /// <param name="serviceId">The service ID</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> ServiceDeregister(string serviceId, CancellationToken ct = default)
        {
            return _client.PutNothing($"/v1/agent/service/deregister/{serviceId}").Execute(ct);
        }

        /// <summary>
        ///     PassTTL is used to set a TTL check to the passing state
        /// </summary>
        /// <param name="checkId">The check ID</param>
        /// <param name="note">An optional, arbitrary string to write to the check status</param>
        public Task PassTtl(string checkId, string note, CancellationToken ct = default)
        {
            return LegacyUpdateTtl(checkId, note, TtlStatus.Pass, ct);
        }

        /// <summary>
        ///     WarnTTL is used to set a TTL check to the warning state
        /// </summary>
        /// <param name="checkId">The check ID</param>
        /// <param name="note">An optional, arbitrary string to write to the check status</param>
        public Task WarnTtl(string checkId, string note, CancellationToken ct = default)
        {
            return LegacyUpdateTtl(checkId, note, TtlStatus.Warn, ct);
        }

        /// <summary>
        ///     FailTTL is used to set a TTL check to the failing state
        /// </summary>
        /// <param name="checkId">The check ID</param>
        /// <param name="note">An optional, arbitrary string to write to the check status</param>
        public Task FailTtl(string checkId, string note, CancellationToken ct = default)
        {
            return LegacyUpdateTtl(checkId, note, TtlStatus.Critical, ct);
        }

        /// <summary>
        ///     UpdateTTL is used to update the TTL of a check
        /// </summary>
        /// <param name="checkId">The check ID</param>
        /// <param name="output">An optional, arbitrary string to write to the check status</param>
        /// <param name="status">The state to set the check to</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> UpdateTtl(string checkId, string output, TtlStatus status,
            CancellationToken ct = default)
        {
            var u = new CheckUpdate
            {
                Status = status.Status,
                Output = output
            };
            return _client.Put($"/v1/agent/check/update/{checkId}", u).Execute(ct);
        }

        /// <summary>
        ///     CheckRegister is used to register a new check with the local agent
        /// </summary>
        /// <param name="check">A check registration object</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> CheckRegister(AgentCheckRegistration check,
            CancellationToken ct = default)
        {
            return _client.Put("/v1/agent/check/register", check).Execute(ct);
        }

        /// <summary>
        ///     CheckDeregister is used to deregister a check with the local agent
        /// </summary>
        /// <param name="checkId">The check ID to deregister</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> CheckDeregister(string checkId, CancellationToken ct = default)
        {
            return _client.PutNothing($"/v1/agent/check/deregister/{checkId}").Execute(ct);
        }

        /// <summary>
        ///     Join is used to instruct the agent to attempt a join to another cluster member
        /// </summary>
        /// <param name="addr">The address to join to</param>
        /// <param name="wan">Join the WAN pool</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Join(string addr, bool wan, CancellationToken ct = default)
        {
            var req = _client.PutNothing($"/v1/agent/join/{addr}");
            if (wan) req.Params["wan"] = "1";
            return req.Execute(ct);
        }

        /// <summary>
        ///     ForceLeave is used to have the agent eject a failed node
        /// </summary>
        /// <param name="node">The node name to remove. An attempt to eject a node that doesn't exist will still be successful</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> ForceLeave(string node, CancellationToken ct = default)
        {
            return _client.PutNothing($"/v1/agent/force-leave/{node}").Execute(ct);
        }


        /// <summary>
        ///     Leave is used to have the agent gracefully leave the cluster and shutdown
        /// </summary>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Leave(string node, CancellationToken ct = default)
        {
            return _client.PutNothing("/v1/agent/leave").Execute(ct);
        }

        /// <summary>
        ///     Reload triggers a configuration reload for the agent we are connected to.
        /// </summary>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> Reload(string node, CancellationToken ct = default)
        {
            return _client.PutNothing("/v1/agent/reload").Execute(ct);
        }

        /// <summary>
        ///     EnableServiceMaintenance toggles service maintenance mode on for the given service ID
        /// </summary>
        /// <param name="serviceId">The service ID</param>
        /// <param name="reason">An optional reason</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> EnableServiceMaintenance(string serviceId, string reason,
            CancellationToken ct = default)
        {
            var req = _client.PutNothing($"/v1/agent/service/maintenance/{serviceId}");
            req.Params["enable"] = "true";
            req.Params["reason"] = reason;
            return req.Execute(ct);
        }

        /// <summary>
        ///     DisableServiceMaintenance toggles service maintenance mode off for the given service ID
        /// </summary>
        /// <param name="serviceId">The service ID</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> DisableServiceMaintenance(string serviceId,
            CancellationToken ct = default)
        {
            var req = _client.PutNothing($"/v1/agent/service/maintenance/{serviceId}");
            req.Params["enable"] = "false";
            return req.Execute(ct);
        }

        /// <summary>
        ///     EnableNodeMaintenance toggles node maintenance mode on for the agent we are connected to
        /// </summary>
        /// <param name="reason">An optional reason</param>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> EnableNodeMaintenance(string reason, CancellationToken ct = default)
        {
            var req = _client.PutNothing("/v1/agent/maintenance");
            req.Params["enable"] = "true";
            req.Params["reason"] = reason;
            return req.Execute(ct);
        }

        /// <summary>
        ///     DisableNodeMaintenance toggles node maintenance mode off for the agent we are connected to
        /// </summary>
        /// <returns>An empty write result</returns>
        public Task<WriteResult> DisableNodeMaintenance(CancellationToken ct = default)
        {
            var req = _client.PutNothing("/v1/agent/maintenance");
            req.Params["enable"] = "false";
            return req.Execute(ct);
        }

        /// <summary>
        ///     Monitor yields log lines to display streaming logs from the agent
        ///     Providing a CancellationToken can be used to close the connection and stop the
        ///     log stream, otherwise the log stream will time out based on the HTTP Client's timeout value.
        /// </summary>
        public async Task<LogStream> Monitor(LogLevel level = default,
            CancellationToken ct = default)
        {
            var req = _client.Get<Stream>("/v1/agent/monitor");
            req.Params["loglevel"] = level.ToString().ToLowerInvariant();
            var res = await req.ExecuteStreaming(ct).ConfigureAwait(false);
            return new LogStream(res.Response);
        }

        private Task<WriteResult> LegacyUpdateTtl(string checkId, string note, TtlStatus status,
            CancellationToken ct = default)
        {
            var request = _client.PutNothing($"/v1/agent/check/{status.LegacyStatus}/{checkId}");
            if (!string.IsNullOrEmpty(note)) request.Params.Add("note", note);
            return request.Execute(ct);
        }

        private class CheckUpdate
        {
            public string Status { get; set; }
            public string Output { get; set; }
        }

        public class LogStream : IEnumerable<Task<string>>, IDisposable
        {
            private readonly Stream _mStream;
            private readonly StreamReader _mStreamreader;

            internal LogStream(Stream s)
            {
                _mStream = s;
                _mStreamreader = new StreamReader(s);
            }

            public void Dispose()
            {
                _mStreamreader.Dispose();
                _mStream.Dispose();
            }

            public IEnumerator<Task<string>> GetEnumerator()
            {
                while (!_mStreamreader.EndOfStream) yield return _mStreamreader.ReadLineAsync();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}