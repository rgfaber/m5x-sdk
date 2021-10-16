using System;
using System.Net;
using M5x.Common;
using M5x.Streams;
using Microsoft.AspNetCore.Http;

namespace M5x.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Xeption ToXeption(this HttpRequest request, string subject, string source, string errorMsg)
        {
            return new Xeption
            {
                Subject = subject,
                Content = request.Body.AsString(),
                ErrorMessage = errorMsg,
                Server = request.Host.Value,
                StatusCode = HttpStatusCode.BadRequest,
                StatusDescription = $"{HttpStatusCode.BadRequest}",
                LastModified = DateTime.UtcNow,
                Method = request.Method,
                ProtocolVersion = request.Protocol,
                ResponseStatus = "Bad Request",
                Source = $"{source}",
                Stack = ""
            };
        }
    }
}