using System;
using M5x.Extensions;

namespace M5x.DEC.Schema.Utils;

public static class AttributeUtils
{
    public static string GetTopic<T>()
    {
        var attrs = (TopicAttribute[])typeof(T).GetCustomAttributes(typeof(TopicAttribute), true);
        return attrs.Length > 0 ? attrs[0].Id : throw new Exception($"No [Topic] Defined on {typeof(T)}!");
    }


    public static string GetDbName<T>()
    {
        var attrs = (DbNameAttribute[])typeof(T).GetCustomAttributes(typeof(DbNameAttribute), true);
        return attrs.Length > 0 ? attrs[0].DbName : throw new Exception($"No [DbName] Defined on {typeof(T)}!");
    }


    public static string GetIdPrefix<T>()
    {
        var attrs = (IDPrefixAttribute[])typeof(T).GetCustomAttributes(typeof(IDPrefixAttribute), true);
        return attrs.Length > 0 ? attrs[0].Prefix : throw new Exception($"No [IDPrefix] Defined on {typeof(T)}!");
    }

    public static string GetEndPoint<T>()
    {
        var attrs = (EndpointAttribute[])typeof(T).GetCustomAttributes(typeof(EndpointAttribute), true);
        return attrs.Length > 0 ? attrs[0].Endpoint : throw new Exception($"No [Endpoint] Defined on {typeof(T)}!");
    }

    public static int GetDbInt32<TReadModel>()
    {
        return GetDbName<TReadModel>()
            .ToInt32();
    }
}