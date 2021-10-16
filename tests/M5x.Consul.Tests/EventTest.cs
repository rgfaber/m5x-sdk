// -----------------------------------------------------------------------
//  <copyright file="EventTest.cs" company="PlayFab Inc">
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
using M5x.Consul.Event;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class EventTest : ConsulTestsBase, IDisposable
    {
        public EventTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public async Task Event_FireList()
        {
            var client = new ConsulClient.ConsulClient();

            var userevent = new UserEvent
            {
                Name = "foo"
            };

            var res = await client.Event.Fire(userevent);

            await Task.Delay(100);

            Assert.NotEqual(TimeSpan.Zero, res.RequestTime);
            Assert.False(string.IsNullOrEmpty(res.Response));

            var events = await client.Event.List();
            Assert.NotEmpty(events.Response);
            Assert.Equal(res.Response, events.Response[events.Response.Length - 1].Id);
            Assert.Equal(client.Event.IdToIndex(res.Response), events.LastIndex);
        }
    }
}