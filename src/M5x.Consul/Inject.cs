using System;
using System.Text;
using M5x.Consul.Chassis;
using M5x.Consul.Interfaces;
using M5x.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Consul;

public static class Inject
{
    public static IServiceCollection AddConsul(this IServiceCollection services)
    {
        services.AddSingleton<IConsulClient, ConsulClient.ConsulClient>(p => new ConsulClient.ConsulClient(
            consulConfig =>
            {
                consulConfig.Address = new Uri(ConsulConfig.Address);
                consulConfig.Datacenter = ConsulConfig.Dc;
            }));
        return services;
    }

    public static IServiceCollection AddUriPool(this IServiceCollection services)
    {
        return services?
            .AddConsul()
            .AddSingleton<IUriPool, UriPool>();
        //.AddSingleton<IUriPool, UriPool>(x =>
        //    new UriPool(x.GetRequiredService<IConsulClient>(), CancellationToken.None));
    }

    public static IServiceCollection AddDisco(this IServiceCollection services)
    {
        return services?
            .AddConsul()
            .AddSingleton<IDiscoInspector, DiscoInspector>();
    }

    public static IApplicationBuilder UseDisco(this IApplicationBuilder app, IApiSettings apiInfo)
    {
        app.Properties.Remove("api");
        app.Properties.Add("api", apiInfo);
        app.Map("/disco", HandleDisco);
        return app;
    }

    private static void HandleDisco(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            if (!(app.Properties["api"] is ApiSettings apiInfo)) return;
            var c = context.RequestServices.GetRequiredService<DiscoInspector>();
            c.RunConsulHealth(app);
            c.RunDiscovery(app, apiInfo.Key);
            c.RunCatalogServices(apiInfo.Key);
            var f = new DiscoReportFormatter(c);
            var j = f.GetOutput();

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(j, Encoding.UTF8);
        });
    }
}