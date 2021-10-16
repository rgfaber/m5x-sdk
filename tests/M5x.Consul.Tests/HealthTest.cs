// -----------------------------------------------------------------------
//  <copyright file="HealthTest.cs" company="PlayFab Inc">
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
using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.Consul.Agent;
using M5x.Consul.Health;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class HealthTest : ConsulTestsBase, IDisposable
    {
        public HealthTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Health_AggregatedStatus()
        {
            var cases = new List<AggregatedStatusResult>
            {
                new() {Name = "empty", Expected = HealthStatus.Passing, Checks = null},
                new()
                {
                    Name = "passing",
                    Expected = HealthStatus.Passing,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Passing}
                    }
                },
                new()
                {
                    Name = "warning",
                    Expected = HealthStatus.Warning,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Warning}
                    }
                },
                new()
                {
                    Name = "critical",
                    Expected = HealthStatus.Critical,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Critical}
                    }
                },
                new()
                {
                    Name = "node_maintenance",
                    Expected = HealthStatus.Maintenance,
                    Checks = new List<HealthCheck>
                    {
                        new() {CheckId = HealthStatus.NodeMaintenance}
                    }
                },
                new()
                {
                    Name = "service_maintenance",
                    Expected = HealthStatus.Maintenance,
                    Checks = new List<HealthCheck>
                    {
                        new() {CheckId = HealthStatus.ServiceMaintenancePrefix + "service"}
                    }
                },
                new()
                {
                    Name = "unknown",
                    Expected = HealthStatus.Passing,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Any}
                    }
                },
                new()
                {
                    Name = "maintenance_over_critical",
                    Expected = HealthStatus.Maintenance,
                    Checks = new List<HealthCheck>
                    {
                        new() {CheckId = HealthStatus.NodeMaintenance},
                        new() {Status = HealthStatus.Critical}
                    }
                },
                new()
                {
                    Name = "critical_over_warning",
                    Expected = HealthStatus.Critical,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Critical},
                        new() {Status = HealthStatus.Warning}
                    }
                },
                new()
                {
                    Name = "warning_over_passing",
                    Expected = HealthStatus.Warning,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Warning},
                        new() {Status = HealthStatus.Passing}
                    }
                },
                new()
                {
                    Name = "lots",
                    Expected = HealthStatus.Warning,
                    Checks = new List<HealthCheck>
                    {
                        new() {Status = HealthStatus.Passing},
                        new() {Status = HealthStatus.Passing},
                        new() {Status = HealthStatus.Warning},
                        new() {Status = HealthStatus.Passing}
                    }
                }
            };
            foreach (var test_case in cases) Assert.Equal(test_case.Expected, test_case.Checks.AggregatedStatus());
        }

        [Fact]
        public async Task Health_Checks()
        {
            var client = new ConsulClient.ConsulClient();
            var svcID = KVTest.GenerateTestKeyName();
            var registration = new AgentServiceRegistration
            {
                Name = svcID,
                Tags = new[] {"bar", "baz"},
                Port = 8000,
                Check = new AgentServiceCheck
                {
                    Ttl = TimeSpan.FromSeconds(15)
                }
            };
            try
            {
                await client.Agent.ServiceRegister(registration);
                var checks = await client.Health.Checks(svcID);
                Assert.NotEqual((ulong) 0, checks.LastIndex);
                Assert.NotEmpty(checks.Response);
            }
            finally
            {
                await client.Agent.ServiceDeregister(svcID);
            }
        }

        [Fact]
        public async Task Health_Node()
        {
            var client = new ConsulClient.ConsulClient();

            var info = await client.Agent.Self();
            var checks = await client.Health.Node((string) info.Response["Config"]["NodeName"]);

            Assert.NotEqual((ulong) 0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        [Fact]
        public async Task Health_Service()
        {
            var client = new ConsulClient.ConsulClient();

            var checks = await client.Health.Service("consul", "", false);
            Assert.NotEqual((ulong) 0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        [Fact]
        public async Task Health_State()
        {
            var client = new ConsulClient.ConsulClient();

            var checks = await client.Health.State(HealthStatus.Any);
            Assert.NotEqual((ulong) 0, checks.LastIndex);
            Assert.NotEmpty(checks.Response);
        }

        private struct AggregatedStatusResult
        {
            public string Name;
            public List<HealthCheck> Checks;
            public HealthStatus Expected;
        }
    }
}