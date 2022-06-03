using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using k8s;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Infra.EventStore;
using M5x.DEC.Infra.Web;
using M5x.Kubernetes;
using M5x.Serilog;
using M5x.Stan;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra;

public static class Inject
{
    public static Stream AsStream(this byte[] input)
    {
        var sOut = new MemoryStream();
        sOut.Write(input, 0, input.Length);
        sOut.Seek(0, 0);
        return sOut;
    }

    [Obsolete("Use this method only if you are certain that your app will run in a K8S cluster")]
    private static void ConfigureNATSOptionsFromK8S(this IServiceCollection services,
        Options options,
        string natsClientTlsSecret = "nats-client-tls",
        string natsCredsSecret = "nats-sys-creds",
        string nameSpace = "default")
    {
        var sp = services.BuildServiceProvider();
        var k8sFact = sp.GetService<IKubernetesFactory>();
        var _logger = sp.GetService<ILogger>();
        var k8s = k8sFact.Build();
        var natsClientTls = k8s.ReadNamespacedSecretAsync(natsClientTlsSecret,
                nameSpace)
            .Result;
        var clientCert = natsClientTls
            .Data
            .FirstOrDefault(x => x.Key == "ca.crt")
            .Value;
        var SysCreds = k8s.ReadNamespacedSecretAsync(natsCredsSecret,
            nameSpace).Result;
        var secret = Encoding.UTF8.GetString(SysCreds
            .Data
            .FirstOrDefault(x => x.Key == "sys.creds")
            .Value);
        options.Servers = new[]
        {
            $"nats://nats.{nameSpace}.svc.cluster.local:4222"
        };
        if (!k8sFact.InCluster)
            options.Servers = new[]
            {
                "nats://localhost:4222"
            };
        options.TLSRemoteCertificationValidationCallback = (sender, certificate, chain, errors) => true;
        options.Secure = true;
        options.AddCertificate(new X509Certificate2(clientCert));
        var handlers = new SourceUserJWTHandler(secret, secret);
        options.SetUserCredentialHandlers(
            handlers.SourceUserJWTEventHandler,
            handlers.SourceUserSignatureHandler);
        options.ClosedEventHandler = (sender, args) =>
        {
            var logMessage = $"[CONN]-CLOSED [{args.Conn.ConnectedUrl}]";
            _logger?.Warning(logMessage);
        };
        options.DisconnectedEventHandler = (sender, args) =>
        {
            args.Conn.ResetStats();
            var logMessage = $"[CONN]-DISCONNECTED [{args.Conn.ConnectedUrl}]";
            _logger?.Warning(logMessage);
        };
        options.ReconnectedEventHandler = (sender, args) =>
        {
            args.Conn.ResetStats();
            var logMessage = $"[CONN]-RECONNECTED [{args.Conn.ConnectedUrl}]";
            _logger?.Warning(logMessage);
        };
        options.ReconnectDelayHandler = (sender, args) =>
        {
            var backoff = args.Attempts * args.Attempts;
            var logMessage = $"[CONN]-RECONNECTING after {backoff}s [Attempt: {args.Attempts}]";
            Thread.Sleep(backoff * 1000);
            _logger?.Warning(logMessage);
        };
        options.AsyncErrorEventHandler = (sender, args) =>
        {
            var logMessage =
                $"[CONN]-ERROR [{args.Conn.ConnectedUrl}] Message: [{args.Error}] Error:[{JsonSerializer.Serialize(args.Subscription)}]";
            _logger?.Error(logMessage);
        };
        options.ServerDiscoveredEventHandler = (sender, args) =>
        {
            var logMessage = $"[SRV]-DISCO [{args.Conn.ConnectedUrl}]";
            _logger?.Information(logMessage);
        };
    }


    public static IServiceCollection AddHttpClientInfra(this IServiceCollection services)
    {
        return services?
            .AddConsoleLogger()
            .AddSingleton<Http.IHttpFactory, Http.HttpFactory>();
    }


    [Obsolete("Use this method only if you are certain that your app will run in a K8S cluster")]
    public static IServiceCollection AddDECClientInfraFromK8S(this IServiceCollection services,
        string natsClientTlsSecret = "nats-client-tls",
        string natsCredsSecret = "nats-sys-creds",
        string nameSpace = "default")
    {
        return services?
            .AddConsoleLogger()
            .AddKubernetes()
            .AddStan(options =>
            {
                services.ConfigureNATSOptionsFromK8S(
                    options,
                    natsClientTlsSecret,
                    natsCredsSecret,
                    nameSpace);
            });
    }

    [Obsolete("Use this method only if you are certain that your app will run in a K8S cluster")]
    public static IServiceCollection AddScopedDECInfraFromK8S(this IServiceCollection services,
        string natsClientTlsSecret = "nats-client-tls",
        string natsCredsSecret = "nats-sys-creds",
        string nameSpace = "default")
    {
        return services?
            .AddConsoleLogger()
            .AddKubernetes()
            .AddScopedEventStore()
            .AddStan(options =>
            {
                services.ConfigureNATSOptionsFromK8S(options,
                    natsClientTlsSecret,
                    natsCredsSecret,
                    nameSpace);
            })
            .AddDECBus();
    }


    /// <summary>
    ///     Please set COUCH_LOCAL_USER, COUCH_LOCAL_PWD,
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCouchClient(this IServiceCollection services)
    {
        return services?
            .AddCouchDB(x =>
            {
                x.UseEndpoint(CouchConfig.CouchEndPoint);
                x.UseBasicAuthentication(CouchConfig.CouchUser, CouchConfig.CouchPwd);
                x.DisableDocumentPluralization();
            });
    }


    public static IServiceCollection AddTransientCouchClient(this IServiceCollection services,
        Action<CouchOptionsBuilder> options)

    {
        return services?
            .AddTransient<ICouchClient, CouchClient>(x => new CouchClient(options));
    }


    [Obsolete("Use this method only if you are certain that your app will run in a K8S cluster")]
    public static IServiceCollection AddSingletonDECInfraFromK8S(this IServiceCollection services,
        string natsClientTlsSecret = "nats-client-tls",
        string natsCredsSecret = "nats-sys-creds",
        string nameSpace = "default")
    {
        return services?
            .AddConsoleLogger()
            .AddKubernetes()
            .AddSingletonEventStore()
            .AddStan(options =>
            {
                services.ConfigureNATSOptionsFromK8S(options,
                    natsClientTlsSecret,
                    natsCredsSecret,
                    nameSpace);
            })
            .AddDECBus();
    }
}