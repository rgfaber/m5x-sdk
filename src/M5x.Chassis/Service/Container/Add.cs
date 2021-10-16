using System.Linq;
using M5x.Chassis.Container;
using M5x.Chassis.Container.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Service.Container
{
    public static class Add
    {
        /// <summary>
        ///     Adds the container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IContainer.</returns>
        public static IContainer AddContainer(this IServiceCollection services)
        {
            var container = services.GetContainer();
            if (container != null) return container;
            container = new NoContainer();
            services.AddSingleton(container);
            return container;
        }


        private static IContainer GetContainer(this IServiceCollection services)
        {
            var d = services
                .FirstOrDefault(x => x.ServiceType == typeof(IContainer));
            return d?.ImplementationInstance as IContainer;
        }
    }
}