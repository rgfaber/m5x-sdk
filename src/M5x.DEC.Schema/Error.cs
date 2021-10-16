using System;
using System.Net;
using System.Text.Json;

namespace M5x.DEC.Schema
{
    public record Error
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Stack { get; set; }
        public string Source { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Server { get; set; }
        public string ProtocolVersion { get; set; }
        public string Method { get; set; }
        public DateTime LastModified { get; set; }
        public string StatusDescription { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseStatus { get; set; }
        public Error InnerError { get; set; }

        public override string ToString()
        {
            return $"ERROR: {JsonSerializer.Serialize(this)}";
        }
    }
}