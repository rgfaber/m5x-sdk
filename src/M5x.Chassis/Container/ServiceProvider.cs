// ***********************************************************************
// <copyright file="GatewayProvider.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using M5x.Chassis.Container.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Container;

/// <summary>
///     Class GatewayProvider. This class cannot be inherited.
/// </summary>
/// <seealso cref="System.IServiceProvider" />
/// <seealso cref="Microsoft.Extensions.DependencyInjection.ISupportRequiredService" />
internal sealed class ServiceProvider : IServiceProvider, ISupportRequiredService
{
    /// <summary>
    ///     The container
    /// </summary>
    private readonly IContainer _container;

    /// <summary>
    ///     The fallback
    /// </summary>
    private readonly IServiceProvider _fallback;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProvider" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="services">The services.</param>
    public ServiceProvider(IContainer container, IServiceCollection services)
    {
        _container = container;
        _fallback = services.BuildServiceProvider();
        RegisterServiceDescriptors(services);
    }

    /// <summary>
    ///     Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    ///     A service object of type <paramref name="serviceType">serviceType</paramref>.   -or-  null if there is no
    ///     service object of type <paramref name="serviceType">serviceType</paramref>.
    /// </returns>
    public object GetService(Type serviceType)
    {
        return _container.Resolve(serviceType) ?? _fallback.GetService(serviceType);
    }

    /// <summary>
    ///     Gets service of type <paramref name="serviceType" /> from the <see cref="T:System.IServiceProvider" /> implementing
    ///     this interface.
    /// </summary>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>
    ///     A service object of type <paramref name="serviceType" />.
    ///     Throws an exception if the <see cref="T:System.IServiceProvider" /> cannot create the object.
    /// </returns>
    public object GetRequiredService(Type serviceType)
    {
        return _container.Resolve(serviceType) ?? _fallback.GetRequiredService(serviceType);
    }

    /// <summary>
    ///     Registers the service descriptors.
    /// </summary>
    /// <param name="services">The services.</param>
    private void RegisterServiceDescriptors(IServiceCollection services)
    {
        // we're going to shell out to the native container for anything passed in here
        foreach (var descriptor in services)
            _container.Register(descriptor.ServiceType, () => _fallback.GetService(descriptor.ServiceType));
    }
}