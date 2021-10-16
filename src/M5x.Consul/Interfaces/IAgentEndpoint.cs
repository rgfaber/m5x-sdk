// -----------------------------------------------------------------------
//  <copyright file="Health.cs" company="PlayFab Inc">>
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        Task<http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Agent;
using M5x.Consul.Client;

namespace M5x.Consul.Interfaces
{
    public interface IAgentEndpoint
    {
        [Obsolete("This property will be removed in 0.8.0. Replace uses of it with a call to GetNodeName()")]
        string NodeName { get; }

        Task<WriteResult> CheckDeregister(string checkId, CancellationToken ct = default);

        Task<WriteResult> CheckRegister(AgentCheckRegistration check,
            CancellationToken ct = default);

        Task<QueryResult<Dictionary<string, AgentCheck>>> Checks(CancellationToken ct = default);
        Task<WriteResult> DisableNodeMaintenance(CancellationToken ct = default);

        Task<WriteResult> DisableServiceMaintenance(string serviceId,
            CancellationToken ct = default);

        Task<WriteResult> EnableNodeMaintenance(string reason, CancellationToken ct = default);

        Task<WriteResult> EnableServiceMaintenance(string serviceId, string reason,
            CancellationToken ct = default);

        Task FailTtl(string checkId, string note, CancellationToken ct = default);
        Task<WriteResult> ForceLeave(string node, CancellationToken ct = default);
        Task<WriteResult> Join(string addr, bool wan, CancellationToken ct = default);
        Task<QueryResult<AgentMember[]>> Members(bool wan, CancellationToken ct = default);
        Task<string> GetNodeName(CancellationToken ct = default);
        Task PassTtl(string checkId, string note, CancellationToken ct = default);

        Task<QueryResult<Dictionary<string, Dictionary<string, dynamic>>>> Self(
            CancellationToken ct = default);

        Task<WriteResult> ServiceDeregister(string serviceId, CancellationToken ct = default);

        Task<WriteResult> ServiceRegister(AgentServiceRegistration service,
            CancellationToken ct = default);

        Task<QueryResult<Dictionary<string, AgentService>>> Services(CancellationToken ct = default);

        Task<WriteResult> UpdateTtl(string checkId, string output, TtlStatus status,
            CancellationToken ct = default);

        Task WarnTtl(string checkId, string note, CancellationToken ct = default);

        Task<Agent.Agent.LogStream> Monitor(LogLevel level = default,
            CancellationToken ct = default);

        Task<WriteResult> Leave(string node, CancellationToken ct = default);
        Task<WriteResult> Reload(string node, CancellationToken ct = default);
    }
}