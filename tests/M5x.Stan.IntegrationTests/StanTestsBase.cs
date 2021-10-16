using System.Threading.Tasks;
using M5x.Kubernetes;
using M5x.Testing;
using NATS.Client;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Stan.IntegrationTests
{
    public abstract class StanTestsBase : IoCTestsBase
    {
        protected INatsClientConnectionFactory ConFact;
        protected IConnection Connection;
        protected IEncodedConnection EncodedConnection;

        public StanTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected IKubernetesFactory K8S { get; set; }


        protected override void Initialize()
        {
            Connection = Container.GetService<IConnection>();
            Assert.NotNull(Connection);
            EncodedConnection = Container.GetService<IEncodedConnection>();
            Assert.NotNull(EncodedConnection);
            ConFact = Container.GetService<INatsClientConnectionFactory>();
            Assert.NotNull(ConFact);
            K8S = Container.GetService<IKubernetesFactory>();
        }
    }
}