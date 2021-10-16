using System.Collections.Generic;

namespace M5x.CEQS.TestKit.Integration
{
    public static class Library
    {
        public static readonly Docker.Models.ContainerInfo CouchDb = new()
        {
            ContainerName = "couchdb-test",
            HostName = "couchdb",
            ImageName = "local/couchdb",
            ImageTag = "",
            Env = new List<string>
            {
                "COUCHDB_USER=root",
                "COUCHDB_PASSWORD=dev"
            },
            Ports = new Dictionary<string, string>
            {
                {"5984", "5984"}
            },
            ExposedPorts = new List<string> {"5984", "4369", "9100"},
            VerifyUrls = new List<string>
            {
                "http://localhost:5984"
            },
            ForceNewImage = false,
            ForceNewContainer = false,
            MaxRestarts = 5
        };

        public static Docker.Models.ContainerInfo Consul = new()
        {
            Cmd = new List<string> {"consul", "agent", "-dev", "-ui", "-client", "0.0.0.0"},
            ContainerName = "consul-test",
            HostName = "consuld",
            ImageName = "library/consul",
            ImageTag = "latest",
            Ports = new Dictionary<string, string>
            {
                {"8300", "8300"},
                {"8400", "8400"},
                {"8500", "8500"},
                {"8600", "8600"}
            },
            ExposedPorts = new List<string>
            {
                "8300",
                "8400",
                "8500",
                "8600"
            },
            VerifyUrls = new List<string>
            {
                "http://localhost:8500"
            },
            ForceNewImage = false,
            ForceNewContainer = false,
            MaxRestarts = 5
        };


        public static Docker.Models.ContainerInfo Nats = new()
        {
            ContainerName = "nats-test",
            //Cmd = new List<string> {"/gnatsd", "-c", "gnatsd.conf", "-m", "8222"},
            HostName = "nats",
            ImageName = "nats",
            ImageTag = "alpine",
            Ports = new Dictionary<string, string>
            {
                {"4222", "4222"},
                {"6222", "6222"},
                {"8222", "8222"}
            },
            ExposedPorts = new List<string> {"4222", "6222", "8222"},
            VerifyUrls = new List<string>
            {
                "http://localhost:8222"
            },
            ForceNewImage = false,
            ForceNewContainer = false,
            MaxRestarts = 5
        };

        public static Docker.Models.ContainerInfo EventStore = new()
        {
            ContainerName = "es-test",
            HostName = "es-test",
            ImageName = "eventstore/eventstore",
            ImageTag = "21.2.0-bionic",
            Ports = new Dictionary<string, string>
            {
                {"1112", "1112"},
                {"2113", "2113"},
            },
            ExposedPorts = new List<string> {"1112", "2113", "8222"},
            VerifyUrls = new List<string>
            {
                "http://localhost:2113"
            },
            ForceNewImage = false,
            ForceNewContainer = false,
            MaxRestarts = 5,
            Env = new List<string> {
                "EVENTSTORE_INSECURE=True",
                "EVENTSTORE_CLUSTER_SIZE=1",
                "EVENTSTORE_DEV=True",
                "EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP=True",
                "EVENTSTORE_INDEX=/usr/data/eventstore-index",
                "EVENTSTORE_LOG=/usr/data/eventstore-log",
                "EVENTSTORE_DB=/usr/data/eventstore-db",
                "EVENTSTORE_RUN_PROJECTIONS=All",
                "EVENTSTORE_START_STANDARD_PROJECTIONS=True",
                "EVENTSTORE_GOSSIP_ALLOWED_DIFFERENCE_MS=600000",
                "EVENTSTORE_INT_TCP_PORT=1111",
                "EVENTSTORE_EXT_TCP_PORT=1112",
                "EVENTSTORE_INT_HTTP_PORT=2112",
                "EVENTSTORE_EXT_HTTP_PORT=2113",
                "EVENTSTORE_DISCOVER_VIA_DNS=False",
                "EVENTSTORE_CLUSTER_DNS=kube-dns",
                "EVENTSTORE_CLUSTER_GOSSIP_PORT=2113"
            }
        };
    }
}