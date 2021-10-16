using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using M5x.Couch.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch
{
    /// <summary>
    ///     A CouchDB HTTP request with all its options. This is where we do the actual HTTP requests to CouchDB.
    /// </summary>
    public class CouchRequest : ICouchRequest
    {
        private const int UploadBufferSize = 100000;
        private readonly ICouchDatabase _db;
        private readonly Dictionary<string, string> _headers = new();
        private readonly ICouchServer _server;
        private string _etag, _etagToCheck;

        // Query options
        private string _method = "GET"; // PUT, DELETE, POST, HEAD
        private string _mimeType;
        private string _path;
        private Stream _postStream;
        private string _query;

        private JToken _result;

        /// <summary>
        ///     Sets the e-tag value
        /// </summary>
        /// <param name="value">The e-tag value</param>
        /// <returns>A CouchRequest with the new e-tag value</returns>
        public ICouchRequest Etag(string value)
        {
            _etagToCheck = value;
            _headers["If-Modified"] = value;
            return this;
        }

        /// <summary>
        ///     Sets the absolute path in the query
        /// </summary>
        /// <param name="name">The absolute path</param>
        /// <returns>A <see cref="CouchRequest" /> with the new path set.</returns>
        public ICouchRequest Path(string name)
        {
            _path = name;
            return this;
        }

        /// <summary>
        ///     Sets the raw query information in the request. Don't forget to start with a question mark (?).
        /// </summary>
        /// <param name="value">The raw query</param>
        /// <returns>A <see cref="CouchRequest" /> with the new query set.</returns>
        public ICouchRequest Query(string value)
        {
            _query = value;
            return this;
        }

        public ICouchRequest QueryOptions(ICollection<KeyValuePair<string, string>> options)
        {
            if (options == null || options.Count == 0) return this;

            var sb = new StringBuilder();
            sb.Append("?");
            foreach (var q in options)
            {
                if (sb.Length > 1) sb.Append("&");
                sb.Append(HttpUtility.UrlEncode(q.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(q.Value));
            }

            return Query(sb.ToString());
        }

        /// <summary>
        ///     Turn the request into a HEAD request, HEAD requests are problematic
        ///     under Mono 2.4, but has been fixed in later releases.
        /// </summary>
        public ICouchRequest Head()
        {
            // NOTE: We need to do this until next release of mono where HEAD requests have been fixed!
            _method = _server.RunningOnMono ? "GET" : "HEAD";
            return this;
        }

        public ICouchRequest Copy()
        {
            _method = "COPY";
            return this;
        }

        public ICouchRequest PostJson()
        {
            MimeTypeJson();
            return Post();
        }

        public ICouchRequest Post()
        {
            _method = "POST";
            return this;
        }

        public ICouchRequest Get()
        {
            _method = "GET";
            return this;
        }

        public ICouchRequest Put()
        {
            _method = "PUT";
            return this;
        }

        public ICouchRequest Delete()
        {
            _method = "DELETE";
            return this;
        }

        public ICouchRequest Data(string data)
        {
            return Data(Encoding.UTF8.GetBytes(data));
        }

        public ICouchRequest Data(byte[] data)
        {
            _postStream = new MemoryStream(data);
            return this;
        }

        public ICouchRequest Data(Stream dataStream)
        {
            _postStream = dataStream;
            return this;
        }

        public ICouchRequest MimeType(string type)
        {
            _mimeType = type;
            return this;
        }

        public ICouchRequest MimeTypeJson()
        {
            MimeType("application/json");
            return this;
        }

        public T Result<T>() where T : JToken
        {
            return (T)_result;
        }

        public string Etag()
        {
            return _etag;
        }

        public ICouchRequest Check(string message)
        {
            try
            {
                if (_result == null) Parse();
                if (!_result["ok"].Value<bool>())
                    throw CouchException.Create(string.Format(CultureInfo.InvariantCulture, message + ": {0}",
                        _result));
                return this;
            }
            catch (WebException e)
            {
                throw CouchException.Create(message, e);
            }
        }

        public T Parse<T>() where T : JToken
        {
            using (var response = GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var textReader = new JsonTextReader(reader))
                        {
                            PickETag(response);
                            if (_etagToCheck != null)
                                if (IsETagValid())
                                    return null;
                            _result = JToken.ReadFrom(textReader); // We know it is a top level JSON JObject.
                        }
                    }
                }
            }

            return (T)_result;
        }

        /// <summary>
        ///     Return the request as a plain string instead of trying to parse it.
        /// </summary>
        public string String()
        {
            using (var response = GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    PickETag(response);
                    if (_etagToCheck != null)
                        if (IsETagValid())
                            return null;
                    return reader.ReadToEnd();
                }
            }
        }

        public WebResponse Response()
        {
            var response = GetResponse();

            PickETag(response);
            if (_etagToCheck != null)
                if (IsETagValid())
                    return null;
            return response;
        }

        public ICouchRequest Send()
        {
            using (var response = GetResponse())
            {
                PickETag(response);
                return this;
            }
        }

        public bool IsETagValid()
        {
            return _etagToCheck == _etag;
        }

        public ICouchRequest AddHeader(string key, string value)
        {
            _headers[key] = value;
            return this;
        }

        public JObject Result()
        {
            return (JObject)_result;
        }

        public JObject Parse()
        {
            return Parse<JObject>();
        }

        /// <summary>
        ///     Returns a Json stream from the server
        /// </summary>
        /// <returns></returns>
        public JsonTextReader Stream()
        {
            return new JsonTextReader(new StreamReader(GetResponse().GetResponseStream()));
        }

        private HttpWebRequest GetRequest()
        {
            var requestUri = new UriBuilder("http", _server.Host, _server.Port,
                (_db != null ? _db.Name + "/" : "") + _path,
                _query).Uri;
            var request = WebRequest.Create(requestUri) as HttpWebRequest;
            if (request == null) throw CouchException.Create("Failed to create request");
            request.Timeout = 3600000; // 1 hour. May use System.Threading.Timeout.Infinite;
            request.Method = _method;

            if (_mimeType != null) request.ContentType = _mimeType;

            foreach (var header in _headers) request.Headers.Add(header.Key, header.Value);

            if (!string.IsNullOrEmpty(_server.EncodedCredentials))
                request.Headers.Add("Authorization", _server.EncodedCredentials);

            if (_postStream != null) WriteData(request);

            // Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Request: {0} Method: {1}", requestUri,
            //     _method));
            return request;
        }

        private void WriteData(HttpWebRequest request)
        {
            request.ContentLength = _postStream.Length;
            using (var ps = request.GetRequestStream())
            {
                var buffer = new byte[UploadBufferSize];
                int bytesRead;
                while ((bytesRead = _postStream.Read(buffer, 0, buffer.Length)) != 0) ps.Write(buffer, 0, bytesRead);
            }
        }

        private void PickETag(WebResponse response)
        {
            _etag = response.Headers["ETag"];
            if (_etag != null) _etag = _etag.EndsWith("\"") ? _etag.Substring(1, _etag.Length - 2) : _etag;
        }

        private WebResponse GetResponse()
        {
            return GetRequest().GetResponse();
        }

        #region Contructors

        public CouchRequest(ICouchServer server)
        {
            _server = server;
        }

        public CouchRequest(ICouchDatabase db)
        {
            _server = db.Server;
            _db = db;
        }

        #endregion
    }
}