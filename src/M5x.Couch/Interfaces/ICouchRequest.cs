using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace M5x.Couch.Interfaces;

public interface ICouchRequest
{
    ICouchRequest AddHeader(string key, string value);
    ICouchRequest Check(string message);
    ICouchRequest Copy();
    ICouchRequest Data(Stream dataStream);
    ICouchRequest Data(string data);
    ICouchRequest Data(byte[] data);
    ICouchRequest Delete();
    string Etag();
    ICouchRequest Etag(string value);
    ICouchRequest Get();
    ICouchRequest Head();
    bool IsETagValid();
    ICouchRequest MimeType(string type);
    ICouchRequest MimeTypeJson();
    T Parse<T>() where T : JToken;
    JObject Parse();
    ICouchRequest Path(string name);
    ICouchRequest Post();
    ICouchRequest PostJson();
    ICouchRequest Put();
    ICouchRequest Query(string name);
    ICouchRequest QueryOptions(ICollection<KeyValuePair<string, string>> options);
    WebResponse Response();
    T Result<T>() where T : JToken;
    JObject Result();
    ICouchRequest Send();
    JsonTextReader Stream();
    string String();
}