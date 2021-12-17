using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using M5x.Consul.Utilities;
using Newtonsoft.Json;

namespace M5x.Consul.Client;

public abstract class ConsulRequest
{
    protected Stopwatch Timer = new();

    internal ConsulRequest(ConsulClient.ConsulClient client, string url, HttpMethod method)
    {
        Client = client;
        Method = method;
        Endpoint = url;

        Params = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(client.Config.Datacenter)) Params["dc"] = client.Config.Datacenter;
        if (client.Config.WaitTime.HasValue) Params["wait"] = client.Config.WaitTime.Value.ToGoDuration();
    }

    internal ConsulClient.ConsulClient Client { get; set; }
    internal HttpMethod Method { get; set; }
    public Dictionary<string, string> Params { get; set; }
    internal Stream ResponseStream { get; set; }
    internal string Endpoint { get; set; }

    protected abstract void ApplyOptions(ConsulClientConfiguration clientConfig);
    protected abstract void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig);

    protected Uri BuildConsulUri(string url, Dictionary<string, string> p)
    {
        var builder = new UriBuilder(Client.Config.Address);
        builder.Path = url;

        ApplyOptions(Client.Config);

        var queryParams = new List<string>(Params.Count / 2);
        foreach (var queryParam in Params)
            if (!string.IsNullOrEmpty(queryParam.Value))
                queryParams.Add($"{Uri.EscapeDataString(queryParam.Key)}={Uri.EscapeDataString(queryParam.Value)}");
            else
                queryParams.Add($"{Uri.EscapeDataString(queryParam.Key)}");

        builder.Query = string.Join("&", queryParams);
        return builder.Uri;
    }

    protected TOut Deserialize<TOut>(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Client.Serializer.Deserialize<TOut>(jsonReader);
            }
        }
    }

    protected byte[] Serialize(object value)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
    }
}