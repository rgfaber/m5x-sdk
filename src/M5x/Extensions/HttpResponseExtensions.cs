using System;
using System.Net.Http;
using M5x.Common;

namespace M5x.Extensions
{
    public static class HttpResponseExtensions
    {
        public static Xeption ToXeption(this HttpResponseMessage resp, string source = null, string server = null)
        {
            if (resp == null) return null;
            var result = new Xeption();
            result.Content = resp.Content.ReadAsStringAsync().Result;
            result.LastModified = DateTime.UtcNow;
            result.Method = $"{resp.RequestMessage.Method}";
            result.ProtocolVersion = $"{resp.RequestMessage.Version}";
            result.ResponseStatus = $"{resp.StatusCode}";
            result.Source = source;
            result.Server = server;
            result.Subject = resp.ReasonPhrase;
            result.StatusCode = resp.StatusCode;
            return result;
        }
    }
}