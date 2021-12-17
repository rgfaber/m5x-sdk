using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.Consul.Client;

public class PostRequest<TIn> : ConsulRequest
{
    private readonly TIn _body;

    public PostRequest(ConsulClient.ConsulClient client, string url, TIn body, WriteOptions options = null) : base(
        client, url,
        HttpMethod.Post)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        _body = body;
        Options = options ?? WriteOptions.Default;
    }

    public WriteOptions Options { get; set; }

    public async Task<WriteResult> Execute(CancellationToken ct)
    {
        Client.CheckDisposed();
        Timer.Start();
        var result = new WriteResult();

        HttpContent content = null;

        if (typeof(TIn) == typeof(byte[]))
        {
            var bodyBytes = _body as byte[];
            if (bodyBytes != null) content = new ByteArrayContent(bodyBytes);
            // If body is null and should be a byte array, then just don't send anything.
        }
        else
        {
            content = new ByteArrayContent(Serialize(_body));
        }

        var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
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