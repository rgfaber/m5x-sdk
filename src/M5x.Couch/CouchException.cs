using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch;

/// <summary>
///     All Exceptions thrown inside Divan uses this class, MOST of these wrap a WebException
///     and we extract the HttpStatusCode to make it easily accessible.
/// </summary>
[Serializable]
public class CouchException : Exception
{
    public HttpStatusCode StatusCode;

    public CouchException()
    {
    }

    public CouchException(string message)
        : base(message)
    {
    }

    public CouchException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CouchException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public static Exception Create(string message)
    {
        return new CouchException(message);
    }

    public static Exception Create(string message, WebException e)
    {
        var msg = string.Format(CultureInfo.InvariantCulture, message + ": {0}", e.Message);
        if (e.Response != null)
        {
            var webResponse = (HttpWebResponse)e.Response;
            // Pick out status code
            var code = webResponse.StatusCode;
            using (var stream = new JsonTextReader(new StreamReader(webResponse.GetResponseStream())))
            {
                // if we don't get a valid {error:, reason:}, don't worry about it
                try
                {
                    var error = JToken.ReadFrom(stream);
                    msg += string.Format(CultureInfo.InvariantCulture, " error: {0}, reason: {1}",
                        error.Value<string>("error"), error.Value<string>("reason"));
                }
                catch
                {
                }

                // Create any specific exceptions we care to use
                if (code == HttpStatusCode.Conflict) return new CouchConflictException(msg, e);
                if (code == HttpStatusCode.NotFound) return new CouchNotFoundException(msg, e);
            }
        }

        // Fall back on generic CouchException
        return new CouchException(msg, e);
    }
}