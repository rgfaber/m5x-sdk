using Newtonsoft.Json.Linq;

namespace M5x.Couch;

/// <summary>
///     This is a view result from a CouchQuery. The result is returned as JSON
///     from CouchDB and parsed into a JObject by Newtonsoft.Json. A view result
///     also includes some meta information and this class has methods to access these.
///     Typically you use a subclass.
/// </summary>
public class CouchViewResult
{
    public string Etag;
    public JObject result;

    public void Result(JObject obj)
    {
        result = obj;
    }

    public int TotalCount()
    {
        return result["total_rows"].Value<int>();
    }

    public int Offset()
    {
        return result["offset"].Value<int>();
    }

    public JEnumerable<JToken> Rows()
    {
        return result["rows"].Children();
    }

    public int Count()
    {
        return result["rows"].Value<JArray>().Count;
    }
}