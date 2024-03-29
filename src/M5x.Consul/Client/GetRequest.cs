﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Utilities;

namespace M5x.Consul.Client;

public class GetRequest<TOut> : ConsulRequest
{
    public GetRequest(ConsulClient.ConsulClient client, string url, QueryOptions options = null) : base(client, url,
        HttpMethod.Get)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        Options = options ?? QueryOptions.Default;
    }

    public QueryOptions Options { get; set; }

    public async Task<QueryResult<TOut>> Execute(CancellationToken ct)
    {
        Client.CheckDisposed();
        Timer.Start();
        var result = new QueryResult<TOut>();

        var message = new HttpRequestMessage(HttpMethod.Get, BuildConsulUri(Endpoint, Params));
        ApplyHeaders(message, Client.Config);
        var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

        ParseQueryHeaders(response, result);
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

    public async Task<QueryResult<Stream>> ExecuteStreaming(CancellationToken ct)
    {
        Client.CheckDisposed();
        Timer.Start();
        var result = new QueryResult<Stream>();

        var message = new HttpRequestMessage(HttpMethod.Get, BuildConsulUri(Endpoint, Params));
        ApplyHeaders(message, Client.Config);
        var response = await Client.HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, ct)
            .ConfigureAwait(false);

        ParseQueryHeaders(response, result as QueryResult<TOut>);
        result.StatusCode = response.StatusCode;
        ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        result.Response = ResponseStream;

        if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            throw new ConsulRequestException($"Unexpected response, status code {response.StatusCode}",
                response.StatusCode);

        result.RequestTime = Timer.Elapsed;
        Timer.Stop();

        return result;
    }

    protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
    {
        if (Options == QueryOptions.Default) return;

        if (!string.IsNullOrEmpty(Options.Datacenter)) Params["dc"] = Options.Datacenter;
        switch (Options.Consistency)
        {
            case ConsistencyMode.Consistent:
                Params["consistent"] = string.Empty;
                break;
            case ConsistencyMode.Stale:
                Params["stale"] = string.Empty;
                break;
            case ConsistencyMode.Default:
                break;
        }

        if (Options.WaitIndex != 0) Params["index"] = Options.WaitIndex.ToString();
        if (Options.WaitTime.HasValue) Params["wait"] = Options.WaitTime.Value.ToGoDuration();
        if (!string.IsNullOrEmpty(Options.Near)) Params["near"] = Options.Near;
    }

    protected void ParseQueryHeaders(HttpResponseMessage res, QueryResult<TOut> meta)
    {
        var headers = res.Headers;

        if (headers.Contains("X-Consul-Index"))
            try
            {
                meta.LastIndex = ulong.Parse(headers.GetValues("X-Consul-Index").First());
            }
            catch (Exception ex)
            {
                throw new ConsulRequestException("Failed to parse X-Consul-Index", res.StatusCode, ex);
            }

        if (headers.Contains("X-Consul-LastContact"))
            try
            {
                meta.LastContact =
                    TimeSpan.FromMilliseconds(ulong.Parse(headers.GetValues("X-Consul-LastContact").First()));
            }
            catch (Exception ex)
            {
                throw new ConsulRequestException("Failed to parse X-Consul-LastContact", res.StatusCode, ex);
            }

        if (headers.Contains("X-Consul-KnownLeader"))
            try
            {
                meta.KnownLeader = bool.Parse(headers.GetValues("X-Consul-KnownLeader").First());
            }
            catch (Exception ex)
            {
                throw new ConsulRequestException("Failed to parse X-Consul-KnownLeader", res.StatusCode, ex);
            }

        if (headers.Contains("X-Consul-Translate-Addresses"))
            try
            {
                meta.AddressTranslationEnabled =
                    bool.Parse(headers.GetValues("X-Consul-Translate-Addresses").First());
            }
            catch (Exception ex)
            {
                throw new ConsulRequestException("Failed to parse X-Consul-Translate-Addresses", res.StatusCode,
                    ex);
            }
    }

    protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
    {
        if (!string.IsNullOrEmpty(Options.Token)) message.Headers.Add("X-Consul-Token", Options.Token);
    }
}