using System;

namespace M5x.Consul.Session;

public class SessionBehavior : IEquatable<SessionBehavior>
{
    public string Behavior { get; private set; }

    public static SessionBehavior Release { get; } = new() { Behavior = "release" };

    public static SessionBehavior Delete { get; } = new() { Behavior = "delete" };

    public bool Equals(SessionBehavior other)
    {
        return other != null && Behavior.Equals(other.Behavior);
    }

    public override bool Equals(object other)
    {
        // other could be a reference type, the is operator will return false if null
        var a = other as SessionBehavior;
        return a != null && Equals(a);
    }

    public override int GetHashCode()
    {
        return Behavior.GetHashCode();
    }
}