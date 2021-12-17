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

using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Session;

namespace M5x.Consul.Interfaces;

public interface ISessionEndpoint
{
    Task<WriteResult<string>> Create(CancellationToken ct = default);
    Task<WriteResult<string>> Create(SessionEntry se, CancellationToken ct = default);

    Task<WriteResult<string>> Create(SessionEntry se, WriteOptions q,
        CancellationToken ct = default);

    Task<WriteResult<string>> CreateNoChecks(CancellationToken ct = default);
    Task<WriteResult<string>> CreateNoChecks(SessionEntry se, CancellationToken ct = default);

    Task<WriteResult<string>> CreateNoChecks(SessionEntry se, WriteOptions q,
        CancellationToken ct = default);

    Task<WriteResult<bool>> Destroy(string id, CancellationToken ct = default);
    Task<WriteResult<bool>> Destroy(string id, WriteOptions q, CancellationToken ct = default);
    Task<QueryResult<SessionEntry>> Info(string id, CancellationToken ct = default);

    Task<QueryResult<SessionEntry>> Info(string id, QueryOptions q,
        CancellationToken ct = default);

    Task<QueryResult<SessionEntry[]>> List(CancellationToken ct = default);
    Task<QueryResult<SessionEntry[]>> List(QueryOptions q, CancellationToken ct = default);
    Task<QueryResult<SessionEntry[]>> Node(string node, CancellationToken ct = default);

    Task<QueryResult<SessionEntry[]>> Node(string node, QueryOptions q,
        CancellationToken ct = default);

    Task<WriteResult<SessionEntry>> Renew(string id, CancellationToken ct = default);

    Task<WriteResult<SessionEntry>> Renew(string id, WriteOptions q,
        CancellationToken ct = default);

    Task RenewPeriodic(TimeSpan initialTtl, string id, CancellationToken ct);
    Task RenewPeriodic(TimeSpan initialTtl, string id, WriteOptions q, CancellationToken ct);
}