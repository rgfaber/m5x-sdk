using System;
using Newtonsoft.Json;

namespace M5x.Consul.KV;

[JsonConverter(typeof(KvTxnVerbTypeConverter))]
public class KVTxnVerb : IEquatable<KVTxnVerb>
{
    public static KVTxnVerb Set { get; } = new() { Operation = "set" };

    public static KVTxnVerb Delete { get; } = new() { Operation = "delete" };

    public static KVTxnVerb DeleteCas { get; } = new() { Operation = "delete-cas" };

    public static KVTxnVerb DeleteTree { get; } = new() { Operation = "delete-tree" };

    public static KVTxnVerb Cas { get; } = new() { Operation = "cas" };

    public static KVTxnVerb Lock { get; } = new() { Operation = "lock" };

    public static KVTxnVerb Unlock { get; } = new() { Operation = "unlock" };

    public static KVTxnVerb Get { get; } = new() { Operation = "get" };

    public static KVTxnVerb GetTree { get; } = new() { Operation = "get-tree" };

    public static KVTxnVerb CheckSession { get; } = new() { Operation = "check-session" };

    public static KVTxnVerb CheckIndex { get; } = new() { Operation = "check-index" };

    public string Operation { get; private set; }

    public bool Equals(KVTxnVerb other)
    {
        return Operation == other.Operation;
    }

    public override bool Equals(object other)
    {
        // other could be a reference type, the is operator will return false if null
        return other is KVTxnVerb && Equals(other as KVTxnVerb);
    }

    public override int GetHashCode()
    {
        return Operation.GetHashCode();
    }
}