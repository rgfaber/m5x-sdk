// ***********************************************************************
// <copyright file="NoContainer.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using M5x.Chassis.Compiler;
using M5x.Chassis.Container.Interfaces;

namespace M5x.Chassis.Container;

/// <summary>
///     Class NoContainer.
/// </summary>
/// <seealso cref="IContainer" />
public partial class NoContainer : IContainer
{
    /// <summary>
    ///     The fall back assemblies
    /// </summary>
    private readonly IEnumerable<Assembly> _fallBackAssemblies;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GatewayContainer" /> class.
    /// </summary>
    /// <param name="fallbackAssemblies">The fallback assemblies.</param>
    public NoContainer(IEnumerable<Assembly> fallbackAssemblies = null)
    {
        _fallBackAssemblies = fallbackAssemblies ?? Enumerable.Empty<Assembly>();
        _factory = new InstanceFactory();
    }

    /// <summary>
    ///     Gets or sets a value indicating whether [throw if cant resolve].
    /// </summary>
    /// <value><c>true</c> if [throw if cant resolve]; otherwise, <c>false</c>.</value>
    public bool ThrowIfCantResolve { get; set; }

    /// <summary>
    ///     Disposes this instance.
    /// </summary>
    public void Dispose()
    {
        // No scopes, so nothing to dispose
    }

    #region Register

    /// <summary>
    ///     Struct NameAndType
    /// </summary>
    public struct NameAndType
    {
        /// <summary>
        ///     The type
        /// </summary>
        public readonly Type Type;

        /// <summary>
        ///     The name
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NameAndType" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public NameAndType(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        ///     Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Equals(NameAndType other)
        {
            return Type == other.Type && string.Equals(Name, other.Name);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is NameAndType && Equals((NameAndType)obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type?.GetHashCode() ?? 0) * 397) ^ (Name?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        ///     Class TypeNameEqualityComparer. This class cannot be inherited.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEqualityComparer{macula.io.Fx.Container.GatewayContainer.NameAndType}" />
        private sealed class TypeNameEqualityComparer : IEqualityComparer<NameAndType>
        {
            /// <summary>
            ///     Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type T to compare.</param>
            /// <param name="y">The second object of type T to compare.</param>
            /// <returns>true if the specified objects are equal; otherwise, false.</returns>
            public bool Equals(NameAndType x, NameAndType y)
            {
                return x.Type == y.Type && string.Equals(x.Name, y.Name);
            }

            /// <summary>
            ///     Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public int GetHashCode(NameAndType obj)
            {
                unchecked
                {
                    return ((obj.Type?.GetHashCode() ?? 0) * 397) ^ (obj.Name?.GetHashCode() ?? 0);
                }
            }
        }

        /// <summary>
        ///     Gets the type name comparer.
        /// </summary>
        /// <value>The type name comparer.</value>
        public static IEqualityComparer<NameAndType> TypeNameComparer { get; } = new TypeNameEqualityComparer();
    }

    /// <summary>
    ///     The registrations
    /// </summary>
    private readonly IDictionary<Type, Func<object>>
        _registrations = new ConcurrentDictionary<Type, Func<object>>();

    /// <summary>
    ///     The named registrations
    /// </summary>
    private readonly IDictionary<NameAndType, Func<object>> _namedRegistrations =
        new ConcurrentDictionary<NameAndType, Func<object>>();

    /// <summary>
    ///     The collection registrations
    /// </summary>
    private readonly IDictionary<Type, List<Func<object>>> _collectionRegistrations =
        new ConcurrentDictionary<Type, List<Func<object>>>();

    /// <summary>
    ///     Registers the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    public void Register(Type type, Func<object> builder, Lifetime lifetime = Lifetime.AlwaysNew)
    {
        var next = WrapLifeCycle(builder, lifetime);
        if (_registrations.ContainsKey(type))
        {
            var previous = _registrations[type];
            _registrations[type] = next;
            RegisterManyUnnamed(type, previous);
        }
        else
        {
            _registrations[type] = next;
        }
    }

    /// <summary>
    ///     Registers the specified builder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    public void Register<T>(Func<T> builder, Lifetime lifetime = Lifetime.AlwaysNew) where T : class
    {
        var type = typeof(T);
        Func<object> next = WrapLifeCycle(builder, lifetime);
        if (_registrations.ContainsKey(type))
        {
            var previous = _registrations[type];
            _registrations[type] = next;
            RegisterManyUnnamed(type, previous);
        }
        else
        {
            _registrations[type] = next;
        }
    }

    /// <summary>
    ///     Registers the specified name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    public void Register<T>(string name, Func<T> builder, Lifetime lifetime = Lifetime.AlwaysNew) where T : class
    {
        var type = typeof(T);
        _namedRegistrations[new NameAndType(name, type)] = WrapLifeCycle(builder, lifetime);
    }

    /// <summary>
    ///     Registers the specified name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    public void Register<T>(string name, Func<IDependencyResolver, T> builder,
        Lifetime lifetime = Lifetime.AlwaysNew) where T : class
    {
        var type = typeof(T);
        _namedRegistrations[new NameAndType(name, type)] = () => WrapLifeCycle(builder, lifetime)(this);
    }

    /// <summary>
    ///     Registers the specified builder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    public void Register<T>(Func<IDependencyResolver, T> builder, Lifetime lifetime = Lifetime.AlwaysNew)
        where T : class
    {
        var type = typeof(T);
        Func<object> next = () => WrapLifeCycle(builder, lifetime)(this);
        if (_registrations.ContainsKey(type))
        {
            var previous = _registrations[type];
            _registrations[type] = next;
            RegisterManyUnnamed(type, previous);
        }
        else
        {
            _registrations[type] = next;
        }
    }

    /// <summary>
    ///     Registers the specified instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    public void Register<T>(T instance)
    {
        var type = typeof(T);
        Func<object> next = () => instance;
        if (_registrations.ContainsKey(type))
        {
            var previous = _registrations[type];
            _registrations[type] = next;
            RegisterManyUnnamed(type, previous);
        }
        else
        {
            _registrations[type] = next;
        }
    }

    /// <summary>
    ///     Registers the many unnamed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="previous">The previous.</param>
    private void RegisterManyUnnamed(Type type, Func<object> previous)
    {
        if (!_collectionRegistrations.TryGetValue(type, out var collectionBuilder))
        {
            collectionBuilder = new List<Func<object>> { previous };
            _collectionRegistrations.Add(type, collectionBuilder);
        }

        collectionBuilder.Add(_registrations[type]);

        // implied registration of the enumerable equivalent
        Register(typeof(IEnumerable<>).MakeGenericType(type), () =>
        {
            var collection = (IList)_factory.CreateInstance(typeof(List<>).MakeGenericType(type));
            foreach (var item in YieldCollection(collectionBuilder))
                collection.Add(item);
            return collection;
        }, Lifetime.Permanent);
    }

    #endregion

    #region Resolve

    /// <summary>
    ///     Resolves this instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public T Resolve<T>() where T : class
    {
        var serviceType = typeof(T);
        if (!_registrations.TryGetValue(serviceType, out var builder))
            return AutoResolve(serviceType) as T;
        var resolved = builder() as T;
        if (resolved != null)
            return resolved;
        if (ThrowIfCantResolve)
            throw new InvalidOperationException($"No registration for {serviceType}");
        return null;
    }

    /// <summary>
    ///     Resolves all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    public IEnumerable<T> ResolveAll<T>() where T : class
    {
        var serviceType = typeof(T);
        return _collectionRegistrations.TryGetValue(serviceType, out var collectionBuilder)
            ? YieldCollection<T>(collectionBuilder)
            : Enumerable.Empty<T>();
    }


    /// <summary>
    ///     Yields the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionBuilder">The collection builder.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    private static IEnumerable<T> YieldCollection<T>(IEnumerable<Func<object>> collectionBuilder) where T : class
    {
        foreach (var builder in collectionBuilder)
            yield return builder() as T;
    }

    /// <summary>
    ///     Resolves the specified service type.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public object Resolve(Type serviceType)
    {
        if (!_registrations.TryGetValue(serviceType, out var builder))
            return AutoResolve(serviceType);
        var resolved = builder();
        if (resolved != null)
            return resolved;
        if (ThrowIfCantResolve)
            throw new InvalidOperationException($"No registration for {serviceType}");
        return null;
    }

    /// <summary>
    ///     Resolves all.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>IEnumerable.</returns>
    public IEnumerable ResolveAll(Type serviceType)
    {
        return _collectionRegistrations.TryGetValue(serviceType, out var collectionBuilder)
            ? YieldCollection(collectionBuilder)
            : Enumerable.Empty<object>();
    }


    /// <summary>
    ///     Yields the collection.
    /// </summary>
    /// <param name="collectionBuilder">The collection builder.</param>
    /// <returns>IEnumerable.</returns>
    private static IEnumerable YieldCollection(IEnumerable<Func<object>> collectionBuilder)
    {
        foreach (var builder in collectionBuilder)
            yield return builder();
    }

    /// <summary>
    ///     Resolves the specified name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name.</param>
    /// <returns>T.</returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public T Resolve<T>(string name) where T : class
    {
        if (_namedRegistrations.TryGetValue(new NameAndType(name, typeof(T)), out var builder))
            return builder() as T;
        if (ThrowIfCantResolve)
            throw new InvalidOperationException($"No registration for {typeof(T)} named {name}");
        return null;
    }

    /// <summary>
    ///     Resolves the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public object Resolve(string name, Type serviceType)
    {
        if (_namedRegistrations.TryGetValue(new NameAndType(name, serviceType), out var builder))
            return builder();
        if (ThrowIfCantResolve)
            throw new InvalidOperationException($"No registration for {serviceType} named {name}");
        return null;
    }

    #endregion

    #region Auto-Resolve w/ Fallback

    /// <summary>
    ///     The factory
    /// </summary>
    private readonly InstanceFactory _factory;

    /// <summary>
    ///     Creates the instance.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <returns>System.Object.</returns>
    private object CreateInstance(Type implementationType)
    {
        // type->constructor
        var ctor = _factory.GetOrCacheConstructorForType(implementationType);

        // constructor->parameters
        var parameters = _factory.GetOrCacheParametersForConstructor(ctor);

        // parameterless ctor
        if (parameters.Length == 0)
            return _factory.CreateInstance(implementationType);

        // auto-resolve widest ctor
        var args = new object[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
            args[i] = AutoResolve(parameters[i].ParameterType);

        return _factory.CreateInstance(implementationType, args);
    }

    /// <summary>
    ///     Automatics the resolve.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public object AutoResolve(Type serviceType)
    {
        while (true)
        {
            // got it:
            if (_registrations.TryGetValue(serviceType, out var creator))
                return creator();

            // want it:
            var typeInfo = serviceType.GetTypeInfo();
            if (!typeInfo.IsAbstract)
                return CreateInstance(serviceType);

            // need it:
            var type = _fallBackAssemblies.SelectMany(s => s.GetTypes())
                .FirstOrDefault(i => serviceType.IsAssignableFrom(i) && !i.GetTypeInfo().IsInterface);
            if (type == null)
            {
                if (ThrowIfCantResolve)
                    throw new InvalidOperationException($"No registration for {serviceType}");

                return null;
            }

            serviceType = type;
        }
    }

    #endregion

    #region Lifetime Management

    /// <summary>
    ///     Wraps the lifecycle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <returns>Func&lt;IDependencyResolver, T&gt;.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">lifetime - null</exception>
    private Func<IDependencyResolver, T> WrapLifeCycle<T>(Func<IDependencyResolver, T> builder, Lifetime lifetime)
        where T : class
    {
        Func<IDependencyResolver, T> registration;
        switch (lifetime)
        {
            case Lifetime.AlwaysNew:
                registration = builder;
                break;
            case Lifetime.Permanent:
                registration = ProcessMemoize(builder);
                break;
            case Lifetime.Thread:
                registration = ThreadMemoize(builder);
                break;
#if SupportsRequests
                case Lifetime.Request:
                    registration = RequestMemoize(builder);
                    break;
#endif
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return registration;
    }

    /// <summary>
    ///     Wraps the lifecycle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <returns>Func&lt;T&gt;.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">lifetime - null</exception>
    private Func<T> WrapLifeCycle<T>(Func<T> builder, Lifetime lifetime) where T : class
    {
        Func<T> registration;
        switch (lifetime)
        {
            case Lifetime.AlwaysNew:
                registration = builder;
                break;
            case Lifetime.Permanent:
                registration = ProcessMemoize(builder);
                break;
            case Lifetime.Thread:
                registration = ThreadMemoize(builder);
                break;
#if SupportsRequests
                case Lifetime.Request:
                    registration = RequestMemoize(builder);
                    break;
#endif
            default:
                throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
        }

        return registration;
    }

    /// <summary>
    ///     Processes the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;T&gt;.</returns>
    private static Func<T> ProcessMemoize<T>(Func<T> f)
    {
        var cache = new ConcurrentDictionary<Type, T>();

        return () => cache.GetOrAdd(typeof(T), v => f());
    }

    /// <summary>
    ///     Threads the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;T&gt;.</returns>
    private static Func<T> ThreadMemoize<T>(Func<T> f)
    {
        var cache = new ThreadLocal<T>(f);

        return () => cache.Value;
    }

    /// <summary>
    ///     Processes the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;IDependencyResolver, T&gt;.</returns>
    private Func<IDependencyResolver, T> ProcessMemoize<T>(Func<IDependencyResolver, T> f)
    {
        var cache = new ConcurrentDictionary<Type, T>();

        return r => cache.GetOrAdd(typeof(T), v => f(this));
    }

    /// <summary>
    ///     Threads the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;IDependencyResolver, T&gt;.</returns>
    private Func<IDependencyResolver, T> ThreadMemoize<T>(Func<IDependencyResolver, T> f)
    {
        var cache = new ThreadLocal<T>(() => f(this));

        return r => cache.Value;
    }

    #endregion
}