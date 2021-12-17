using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.Consul.Client;

public class PutReturningRequest<TOut> : ConsulRequest
{
    public PutReturningRequest(ConsulClient.ConsulClient client, string url, WriteOptions options = null) : base(
        client, url,
        HttpMethod.Put)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        Options = options ?? WriteOptions.Default;
    }

    public WriteOptions Options { get; set; }

    public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
    {
        Client.CheckDisposed();
        Timer.Start();
        var result = new WriteResult<TOut>();

        var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
        ApplyHeaders(message, Client.Config);
        var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

        result.StatusCode = response.StatusCode;

        ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
        {
            if (ResponseStream == null)
                throw new ConsulRequestException($"Unexpected response, status code {response.StatusCode}",
                    response.StatusCode);
            using (var sr = new StreamReader(ResponseStream))
            {
                throw new ConsulRequestException(
                    $"Unexpected response, status code {response.StatusCode}: {sr.ReadToEnd()}",
                    response.StatusCode);
            }
        }

        if (response.IsSuccessStatusCode) result.Response = Deserialize<TOut>(ResponseStream);

        result.RequestTime = Timer.Elapsed;
        Timer.Stop();

        return result;
    }

    protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
    {
        if (Options == WriteOptions.Default) return;

        if (!string.IsNullOrEmpty(Options.Datacenter)) Params["dc"] = Options.Datacenter;
    }

    protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
    {
        if (!string.IsNullOrEmpty(Options.Token)) message.Headers.Add("X-Consul-Token", Options.Token);
    }
}