﻿// -----------------------------------------------------------------------
//  <copyright file="StatusTest.cs" company="PlayFab Inc">
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
using System.Threading.Tasks;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class StatusTest : ConsulTestsBase, IDisposable
    {
        public StatusTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public async Task Status_Leader()
        {
            var client = new ConsulClient.ConsulClient();
            var leader = await client.Status.Leader();
            Assert.False(string.IsNullOrEmpty(leader));
        }

        [Fact]
        public async Task Status_Peers()
        {
            var client = new ConsulClient.ConsulClient();
            var peers = await client.Status.Peers();
            Assert.True(peers.Length > 0);
        }
    }
}