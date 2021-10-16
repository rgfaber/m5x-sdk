using System;
using System.Net;
using System.Net.Http;

namespace M5x.Store
{
    public class XResponse
    {
        public string Id { get; set; }
        public string Rev { get; set; }
        public bool IsSuccess { get; set; }
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Error { get; set; }
        public string ETag { get; set; }
        public string Reason { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public HttpMethod RequestMethod { get; set; }
        public Uri RequestUri { get; set; }
        public string DbName { get; set; }
    }
}