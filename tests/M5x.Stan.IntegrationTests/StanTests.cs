using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using M5x.Kubernetes;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Stan.IntegrationTests;

public class StanTests : StanTestsBase
{
    public StanTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public async Task Should_Connect()
    {
    }


    [Fact]
    public async Task Should_Publish()
    {
        var readme = File.ReadAllText("./README.MD");
        EncodedConnection.OnSerialize = o => Encoding.UTF8.GetBytes(o as string);
        EncodedConnection.Publish("WeLoveBeer", readme);
    }

    [Fact]
    public async Task Should_Subscribe()
    {
    }


    [Fact]
    public async Task Should_ContainConnection()
    {
        var conn = Container.GetService<IConnection>();
        Assert.NotNull(conn);
    }

    [Fact]
    public async Task Should_ContainEncodedConnection()
    {
        var conn = Container.GetService<IEncodedConnection>();
        Assert.NotNull(conn);
    }

    protected override void SetTestEnvironment()
    {
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services.AddKubernetes();
        var k8sFact = Container.GetService<IKubernetesFactory>();
        var k8s = k8sFact.Build();

        var natsClientTls = k8s.ReadNamespacedSecretWithHttpMessagesAsync("nats-client-tls", "default").Result;
        var clientCert = natsClientTls
            .Body
            .Data
            .FirstOrDefault(x => x.Key == "ca.crt")
            .Value;
        var SysCreds = k8s.ReadNamespacedSecretWithHttpMessagesAsync("nats-sys-creds", "default").Result;
        var secret = SysCreds
            .Body
            .Data
            .FirstOrDefault(x => x.Key == "sys.creds")
            .Value;
        var secretStr = Encoding.UTF8.GetString(secret);
        services.AddStan(options =>
        {
            options.User = "test";
            options.Name = "TestConnection";
            options.Servers = new[]
            {
                "nats://localhost:4222",
                "nats://nats.local:4222"
            };
            options.TLSRemoteCertificationValidationCallback = (sender, certificate, chain, errors) => true;
            options.Secure = true;
            options.AddCertificate(new X509Certificate2(clientCert));
            var handlers = new SourceUserJWTHandler(secretStr, secretStr);
            options.SetUserCredentialHandlers(
                handlers.SourceUserJWTEventHandler,
                handlers.SourceUserSignatureHandler);
        });
    }

    public async Task Should_AuthenticateUsingSysCredsFromKubernetes()
    {
    }


    [Fact]
    public async Task Should_GetCredentialsFromKubernetes()
    {
        var clt = K8S.Build();
        var cred = await clt.ReadNamespacedSecretWithHttpMessagesAsync("nats-sys-creds", "default");
        var secret = cred.Body;
        var dict = secret.Data;
        foreach (var kvp in dict)
        {
            var val = Encoding.UTF8.GetString(kvp.Value);
            Output.WriteLine($"{kvp.Key} => {val}");
        }
    }
}