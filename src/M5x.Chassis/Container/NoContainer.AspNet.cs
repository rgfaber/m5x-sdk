using System;
using System.Collections.Generic;
using M5x.Chassis.Container.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Container;

public partial class NoContainer : IContainer
{
    /// <summary>
    ///     Requests the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;T&gt;.</returns>
    private Func<T> RequestMemoize<T>(Func<T> f)
    {
        return () =>
        {
            var accessor = Resolve<IHttpContextAccessor>();
            if (accessor?.HttpContext == null)
                return f(); // always new

            var cache = accessor.HttpContext.Items;
            var cacheKey = f.ToString();
            if (cache.TryGetValue(cacheKey, out var item))
                return (T)item; // got it

            item = f(); // need it
            cache.Add(cacheKey, item);
            return (T)item;
        };
    }

    /// <summary>
    ///     Requests the memoize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="f">The f.</param>
    /// <returns>Func&lt;IDependencyResolver, T&gt;.</returns>
    private Func<IDependencyResolver, T> RequestMemoize<T>(Func<IDependencyResolver, T> f)
    {
        return r =>
        {
            var accessor = r.Resolve<IHttpContextAccessor>();
            if (accessor?.HttpContext == null)
                return f(this); // always new

            var cache = accessor.HttpContext.Items;
            var cacheKey = f.ToString();
            if (cache.TryGetValue(cacheKey, out var item))
                return (T)item; // got it

            item = f(this); // need it
            cache.Add(cacheKey, item);
            return (T)item;
        };
    }

    /// <summary>
    ///     Populates the specified services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>IServiceProvider.</returns>
    public IServiceProvider Populate(IServiceCollection services)
    {
        Register<IServiceProvider>(() => new ServiceProvider(this, services), Lifetime.Permanent);
        Register<IServiceScopeFactory>(() => new ServiceScopeFactory(this), Lifetime.Permanent);
        Register<IEnumerable<ServiceDescriptor>>(services);
        Register(this);
        return Resolve<IServiceProvider>();
    }
}