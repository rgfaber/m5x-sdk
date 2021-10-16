using System;
using M5x.DEC.Infra;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Infra.EventStore;
using Microsoft.Extensions.DependencyInjection;
using Robby.Domain;

namespace Robby.Infra
{
    public static class InjectInfra
    {
        
        public static IServiceCollection AddRoboSimActors(this IServiceCollection services)
        {
            return services?
                .AddTransient<InitializeSim.IActor, InitializeSim.Actor>();
        }

        public static IServiceCollection AddRoboSimCmdInfra(this IServiceCollection services)
        {
            return services?
                .AddSingletonDECInfraFromK8S()
                .AddRoboSimActors()
                .AddRoboSimEmitters()
                .AddRoboSimResponders()
                .AddRoboSimEventRepo();
        }


        public static IServiceCollection AddRoboSimEtlInfra(this IServiceCollection services)
        {
            return services?
                .AddScopedDECInfraFromK8S()
                .AddRoboSimStore()
                .AddRoboSimWriters()
                .AddRoboSimListeners();
        }

        public static IServiceCollection AddRoboSimWriters(this IServiceCollection services)
        {
            return services?
                .AddTransient<InitializeSim.IWriter, InitializeSim.Writer>();
        }

        public static IServiceCollection AddRoboSimStore(this IServiceCollection services)
        {
            return services
                .AddTransientCouchClient()
                .AddTransient<IRoboSimStore, RoboSimStore>();
        }
        

        public static IServiceCollection AddRoboSimEventRepo(this IServiceCollection services)
        {
            return services?
                .AddTransient<IEventRepo, EventRepo>();
        }
        public static IServiceCollection AddRoboSimListeners(this IServiceCollection services)
        {
            return services?
                .AddHostedService<InitializeSim.Listener>();
        }

        public static IServiceCollection AddRoboSimResponders(this IServiceCollection services)
        {
            return services?
                .AddHostedService<InitializeSim.Responder>();
        }


        public static IServiceCollection AddRoboSimEmitters(this IServiceCollection services)
        {
            return services?
                .AddTransient<InitializeSim.IEmitter, InitializeSim.Emitter>();
        }
        
        
        
        
        
    }
}
