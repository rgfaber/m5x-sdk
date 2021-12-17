using System.Collections.Generic;
using System.Linq;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC;

public static class Inject
{
    public static IServiceCollection AddDECBus(this IServiceCollection services)
    {
        return services?
            .AddTransient<IDECBus, DECBus>();
        // .AddTransient<IAggregatePublisher, DECBus>()
        // .AddTransient<IAggregateSubscriber, DECBus>();
    }


    public static IServiceCollection RegisterEventHandlers<TAggregateId, TEvent>(this IServiceCollection services,
        IEnumerable<IEventHandler<TAggregateId, TEvent>> handlers)
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
    {
        if (handlers == null) return services;
        if (!handlers.Any()) return services;
        ;
        foreach (var eventHandler in handlers)
        {
            var t = eventHandler.GetType();
            services.AddTransient(o => eventHandler);
        }

        return services;
    }
}