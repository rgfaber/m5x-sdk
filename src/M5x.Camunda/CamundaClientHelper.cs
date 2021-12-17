using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using M5x.Camunda.Transfer;

namespace M5x.Camunda;

public class CamundaClientHelper
{
    public const string CONTENT_TYPE_JSON = "application/json";

    private static HttpClient _client;

    public CamundaClientHelper(Uri restUrl, string username, string password)
    {
        RestUrl = restUrl;
        RestUsername = username;
        RestPassword = password;
    }

    public Uri RestUrl { get; }
    public string RestUsername { get; }
    public string RestPassword { get; }

    public HttpClient HttpClient()
    {
        if (_client != null) return _client;
        if (RestUsername != null)
        {
            var credentials = new NetworkCredential(RestUsername, RestPassword);
            _client = new HttpClient(new HttpClientHandler { Credentials = credentials });
        }
        else
        {
            _client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite) };
            // Infinite / really?
        }

        // Add an Accept header for JSON format.
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE_JSON));
        _client.BaseAddress = RestUrl;
        return _client;
    }

    public static Dictionary<string, Variable> ConvertVariables(Dictionary<string, object> variables)
    {
        // report successful execution
        var result = new Dictionary<string, Variable>();
        if (variables == null) return result;
        foreach (var variable in variables)
        {
            var camundaVariable = new Variable
            {
                Value = variable.Value
            };
            result.Add(variable.Key, camundaVariable);
        }

        return result;
    }
}