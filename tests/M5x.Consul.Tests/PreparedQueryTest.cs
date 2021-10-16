﻿using System;
using M5x.Testing;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class PreparedQueryTest : ConsulTestsBase, IDisposable
    {
        //[Fact]
        //public async Task PreparedQuery_Test()
        //{
        //    var client = new ConsulClient.ConsulClient();

        //    var registration = new CatalogRegistration
        //    {
        //        Datacenter = "dc1",
        //        Node = "foobaz",
        //        Address = "192.168.10.10",
        //        Service = new AgentService
        //        {
        //            ID = "sql1",
        //            Service = "sql",
        //            Tags = new[] {"master", "v1"},
        //            Port = 8000
        //        }
        //    };

        //    await client.Catalog.Register(registration);

        //    Assert.NotNull((await client.Catalog.Node("foobaz")).Response);

        //    var mgmtquerytoken = new QueryOptions {Token = "yep"};

        //    var def = new PreparedQueryDefinition {Service = new ServiceQuery {Service = "sql"}};

        //    var id = (await client.PreparedQuery.Create(def)).Response;
        //    def.ID = id;

        //    var defs = (await client.PreparedQuery.Get(id)).Response;

        //    Assert.NotNull(defs);
        //    Assert.True(defs.Length == 1);
        //    Assert.Equal(def.Service.Service, defs[0].Service.Service);

        //    defs = null;
        //    defs = (await client.PreparedQuery.List(mgmtquerytoken)).Response;

        //    Assert.NotNull(defs);
        //    Assert.True(defs.Length == 1);
        //    Assert.Equal(def.Service.Service, defs[0].Service.Service);

        //    def.Name = "my-query";

        //    await client.PreparedQuery.Update(def);

        //    defs = null;
        //    defs = (await client.PreparedQuery.Get(id)).Response;

        //    Assert.NotNull(defs);
        //    Assert.True(defs.Length == 1);
        //    Assert.Equal(def.Name, defs[0].Name);

        //    var results = (await client.PreparedQuery.Execute(id)).Response;

        //    Assert.NotNull(results);
        //    var nodes = results.Nodes.Where(n => n.Node.Name == "foobaz").ToArray();
        //    Assert.True(nodes.Length == 1);
        //    Assert.Equal(nodes[0].Node.Name, "foobaz");

        //    results = null;
        //    results = (await client.PreparedQuery.Execute("my-query")).Response;

        //    Assert.NotNull(results);
        //    nodes = results.Nodes.Where(n => n.Node.Name == "foobaz").ToArray();
        //    Assert.True(nodes.Length == 1);
        //    Assert.Equal(results.Nodes[0].Node.Name, "foobaz");

        //    await client.PreparedQuery.Delete(id);

        //    defs = null;
        //    defs = (await client.PreparedQuery.List(mgmtquerytoken)).Response;

        //    Assert.True(defs.Length == 0);
        //}

        public PreparedQueryTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
    }
}