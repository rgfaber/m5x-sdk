﻿// -----------------------------------------------------------------------
//  <copyright file="ACLTest.cs" company="PlayFab Inc">
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
using System.Net;
using System.Threading.Tasks;
using M5x.Consul.ACL;
using M5x.Consul.Client;
using M5x.Consul.Utilities;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class ACLTest : ConsulTestsBase, IDisposable
    {
        internal const string ConsulRoot = "yep";

        private readonly AsyncReaderWriterLock.Releaser m_lock;
        //[SkippableFact]
        //public async Task ACL_CloneUpdateDestroy()
        //{
        //    Skip.If(string.IsNullOrEmpty(ConsulRoot));

        //    var client = new ConsulClient(c => { c.Token = ConsulRoot; });
        //    var cloneRequest = await client.ACL.Clone(ConsulRoot);
        //    var aclID = cloneRequest.Response;

        //    var aclEntry = await client.ACL.Info(aclID);
        //    aclEntry.Response.Rules = "key \"\" { policy = \"deny\" }";
        //    await client.ACL.Update(aclEntry.Response);

        //    var aclEntry2 = await client.ACL.Info(aclID);
        //    Assert.Equal("key \"\" { policy = \"deny\" }", aclEntry2.Response.Rules);

        //    var id = cloneRequest.Response;

        //    Assert.NotEqual(TimeSpan.Zero, cloneRequest.RequestTime);
        //    Assert.False(string.IsNullOrEmpty(aclID));

        //    Assert.True((await client.ACL.Destroy(id)).Response);
        //}

        //[SkippableFact]
        //public async Task ACL_Info()
        //{
        //    Skip.If(string.IsNullOrEmpty(ConsulRoot));

        //    var client = new ConsulClient(c => { c.Token = ConsulRoot; });

        //    var aclEntry = await client.ACL.Info(ConsulRoot);

        //    Assert.NotNull(aclEntry.Response);
        //    Assert.NotEqual(TimeSpan.Zero, aclEntry.RequestTime);
        //    Assert.Equal(aclEntry.Response.ID, ConsulRoot);
        //    Assert.Equal(aclEntry.Response.Type, ACLType.Management);
        //}

        //[SkippableFact]
        //public async Task ACL_List()
        //{
        //    Skip.If(string.IsNullOrEmpty(ConsulRoot));

        //    var client = new ConsulClient(c => { c.Token = ConsulRoot; });

        //    var aclList = await client.ACL.List();

        //    Assert.NotNull(aclList.Response);
        //    Assert.NotEqual(TimeSpan.Zero, aclList.RequestTime);
        //    Assert.True(aclList.Response.Length >= 2);
        //}
        public ACLTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
            m_lock = AsyncHelpers.RunSync(SelectiveParallel.Parallel);
        }


        public new void Dispose()
        {
            m_lock.Dispose();
        }

        [Fact]
        public async Task ACL_CreateDestroy()
        {
            try
            {
                var client = new ConsulClient.ConsulClient(c => { c.Token = ConsulRoot; });
                var aclEntry = new ACLEntry
                {
                    Name = "API Test",
                    Type = ACLType.Client,
                    Rules = "key \"\" { policy = \"deny\" }"
                };
                var res = await client.ACL.Create(aclEntry);
                var id = res.Response;

                Assert.NotEqual(TimeSpan.Zero, res.RequestTime);
                Assert.False(string.IsNullOrEmpty(res.Response));

                var aclEntry2 = await client.ACL.Info(id);

                Assert.NotNull(aclEntry2.Response);
                Assert.Equal(aclEntry2.Response.Name, aclEntry.Name);
                Assert.Equal(aclEntry2.Response.Type, aclEntry.Type);
                Assert.Equal(aclEntry2.Response.Rules, aclEntry.Rules);

                Assert.True((await client.ACL.Destroy(id)).Response);
            }
            catch (Exception e)
            {
                if (!(e is ConsulRequestException)) throw;
                if (((ConsulRequestException) e).StatusCode == HttpStatusCode.Unauthorized)
                    return;
                throw;
            }
        }
    }
}