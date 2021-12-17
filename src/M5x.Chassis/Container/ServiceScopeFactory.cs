using System;
using M5x.Chassis.Container.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Container;

/// <summary>
///     Class NoServiceScopeFactory. This class cannot be inherited.
/// </summary>
/// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceScopeFactory" />
internal sealed class ServiceScopeFactory : IServiceScopeFactory
{
    /// <summary>
    ///     The container
    /// </summary>
    private readonly IContainer _container;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceScopeFactory" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public ServiceScopeFactory(IContainer container)
    {
        _container = container;
    }

    /// <summary>
    ///     Create an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> which
    ///     contains an <see cref="T:System.IServiceProvider" /> used to resolve dependencies from a
    ///     newly created scope.
    /// </summary>
    /// <returns>
    ///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> controlling the
    ///     lifetime of the scope. Once this is disposed, any scoped services that have been resolved
    ///     from the <see cref="P:Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider" />
    ///     will also be disposed.
    /// </returns>
    public IServiceScope CreateScope()
    {
        return new GatewayScope(_container);
    }

    /// <summary>
    ///     Class NoServiceScope.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceScope" />
    private class GatewayScope : IServiceScope
    {
        /// <summary>
        ///     The container
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GatewayScope" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public GatewayScope(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     The <see cref="T:System.IServiceProvider" /> used to resolve dependencies from the scope.
        /// </summary>
        /// <value>The service provider.</value>
        public IServiceProvider ServiceProvider => _container.Resolve<IServiceProvider>();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }
    }
}