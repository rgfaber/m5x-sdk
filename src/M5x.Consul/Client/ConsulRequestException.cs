using System;
using System.Net;

namespace M5x.Consul.Client
{
    /// <summary>
    ///     Represents errors that occur while sending data to or fetching data from the Consul agent.
    /// </summary>
    public class ConsulRequestException : Exception
    {
        public ConsulRequestException()
        {
        }

        public ConsulRequestException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public ConsulRequestException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}