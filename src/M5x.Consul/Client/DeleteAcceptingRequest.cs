using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.Consul.Client;

public class DeleteAcceptingRequest<TIn> : ConsulRequest
{
    private readonly TIn _body;

    public DeleteAcceptingRequest(ConsulClient.ConsulClient client, string url, TIn body,
        WriteOptions options = null) : base(
        client, url, HttpMethod.Delete)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        _body = body;
        Options = options ?? WriteOptions.Default;
    }

    public WriteOptions Options { get; set; }

    public async Task<WriteResult> Execute(CancellationToken ct)
    {
        Client.CheckDisposed();
        var result = new WriteResult();

        HttpContent content;

        if (typeof(TIn) == typeof(byte[]))
            content = new ByteArrayContent(_body as byte[] ?? new byte[0]);
        else if (typeof(TIn) == typeof(Stream))
            content = new StreamContent(_body as Stream ?? new MemoryStream());
        else
            content = new ByteArrayContent(Serialize(_body));

        var message = new HttpRequestMessage(HttpMethod.Delete, BuildConsulUri(Endpoint, Params));
        ApplyHeaders(message, Client.Config);
        message.Content = content;

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