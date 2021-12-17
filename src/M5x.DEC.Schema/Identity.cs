using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.ValueObjects;
using M5x.Utils;

namespace M5x.DEC.Schema;

public interface IIdentity
{
    string Value { get; }
}

public abstract record Identity<T> : SingleValueObject<string>, IIdentity
    where T : Identity<T>
{
    public static string GetPrefix()
    {
        var prefixAttributes =
            (IDPrefixAttribute[])typeof(T).GetCustomAttributes(typeof(IDPrefixAttribute), true);
        if (prefixAttributes.Length <= 0) return typeof(T).FullName.Replace(".", "").ToLower();
        var att = prefixAttributes[0];
        return att.Prefix;
    } // ReSharper disable StaticMemberInGenericType

    private static readonly Regex NameReplace = new("Id$");

    // private static readonly string Name = NameReplace.Replace(typeof(T).Name, string.Empty).ToLowerInvariant();
    protected static readonly string Name = GetPrefix();

    private static readonly Regex ValueValidation = new(
        @"^[a-z0-9]+\-(?<guid>[a-f0-9]{8}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{12})$",
        RegexOptions.Compiled);
    // ReSharper enable StaticMemberInGenericType

    public static T New => With(Guid.NewGuid());

    public static T Null => With(Guid.Empty);


    public static T NewDeterministic(Guid namespaceId, string name)
    {
        var guid = GuidFactories.Deterministic.Create(namespaceId, name);
        return With(guid);
    }

    public static T NewDeterministic(Guid namespaceId, byte[] nameBytes)
    {
        var guid = GuidFactories.Deterministic.Create(namespaceId, nameBytes);
        return With(guid);
    }

    public static T NewComb()
    {
        var guid = GuidFactories.Comb.Create();
        return With(guid);
    }

    public static T NewComb(string id)
    {
        if (id.StartsWith(Name))
            id = id.Replace($"{Name}-", "");
        var guid = new Guid(id);
        return With(guid);
    }


    public static T FromGuidString(string id)
    {
        return NewComb(id);
    }


    public static T FromAnyString(string anyString)
    {
        var guid = anyString.AnyStringToGuid();
        return With(guid);
    }

    public static T FromDecimal(decimal value)
    {
        var guid = value.FromDecimal();
        return With(guid);
    }

    public static T With(string value)
    {
        try
        {
            return (T)Activator.CreateInstance(typeof(T), value);
        }
        catch (TargetInvocationException e)
        {
            if (e.InnerException != null) throw e.InnerException;
            throw;
        }
    }


    public static T With(Guid guid)
    {
        var value = $"{Name}-{guid:D}";
        return With(value);
    }

    public static bool IsValid(string value)
    {
        return !Validate(value).Any();
    }

    public static IEnumerable<string> Validate(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            yield return $"Identity of type '{typeof(T).PrettyPrint()}' is null or empty";
            yield break;
        }

        if (!string.Equals(value.Trim(), value, StringComparison.OrdinalIgnoreCase))
            yield return
                $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' contains leading and/or traling spaces";
        if (!value.StartsWith(Name))
            yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not start with '{Name}'";
        if (!ValueValidation.IsMatch(value))
            yield return
                $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not follow the syntax '[NAME]-[GUID]' in lower case";
    }

    protected Identity(string value)
        : base(value)
    {
        if (Config.IdCreationPolicy == IDCreationPolicy.Strict)
        {
            var validationErrors = Validate(value).ToList();
            if (validationErrors.Any())
                throw new ArgumentException($"Identity is invalid: {string.Join(", ", validationErrors)}");
        }

        _lazyGuid = new Lazy<Guid>(() => Guid.Parse(ValueValidation.Match(Value).Groups["guid"].Value));
    }

    // public Identity()
    // {
    //     
    // }

    private readonly Lazy<Guid> _lazyGuid;

    public Guid GetGuid()
    {
        return _lazyGuid.Value;
    }
}