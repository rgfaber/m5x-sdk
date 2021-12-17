using Microsoft.Extensions.DependencyInjection;

namespace M5x.AutoMapper;

public static class Inject
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        return services;
    }
}