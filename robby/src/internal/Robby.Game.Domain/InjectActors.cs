using Microsoft.Extensions.DependencyInjection;

namespace Robby.Game.Domain
{
    public static class InjectActors
    {
        public static IServiceCollection AddGameActors(this IServiceCollection services)
        {
            return services
                .AddInitializeActor()
                .AddUpdateDescriptionActor()
                .AddUpdateDimensionsActor()
                .AddUpdatePopulationActor()
                .AddStartActor();
        }
    }
}