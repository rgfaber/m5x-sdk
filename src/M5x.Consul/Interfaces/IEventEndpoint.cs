﻿// -----------------------------------------------------------------------
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

using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.Event;

namespace M5x.Consul.Interfaces;

public interface IEventEndpoint
{
    Task<WriteResult<string>> Fire(UserEvent ue, CancellationToken ct = default);
    Task<WriteResult<string>> Fire(UserEvent ue, WriteOptions q, CancellationToken ct = default);
    ulong IdToIndex(string uuid);
    Task<QueryResult<UserEvent[]>> List(CancellationToken ct = default);
    Task<QueryResult<UserEvent[]>> List(string name, CancellationToken ct = default);

    Task<QueryResult<UserEvent[]>> List(string name, QueryOptions q,
        CancellationToken ct = default);
}