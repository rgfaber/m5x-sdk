using System;
using System.Linq;
using System.Threading.Tasks;
using M5x.Consul.Agent;
using M5x.Consul.Interfaces;
using M5x.Swagger;
using M5x.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace M5x.Consul.Chassis
{
    public static class Use
    {
        public static IApplicationBuilder UseRegistry(this IApplicationBuilder app, IApplicationLifetime lifeTime,
            IApiSettings api)
        {
            app.Announce(lifeTime, api);
            return app;
        }


        public static async Task<IApplicationBuilder> Announce(this IApplicationBuilder app,
            IApplicationLifetime lifetime,
            IApiSettings api)
        {
            // Retrieve Consul client from DI
            var consulClient = app.ApplicationServices
                .GetRequiredService<IConsulClient>();
            //var consulConfig = app.ApplicationServices
            //    .GetRequiredService<IOptions<ConsulClientConfiguration>>();
            // Setup logger
            var loggingFactory = app.ApplicationServices
                .GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            // Get server IP address
            if (!(app.Properties["server.Features"] is FeatureCollection features)) return app;
            var addresses = features.Get<IServerAddressesFeature>();
            logger.LogInformation("\n---- Found these addresses to bind to  ----\n");
            addresses.PreferHostingUrls = true;
            foreach (var add in addresses.Addresses) logger.LogInformation($"[{add}]");
            logger.LogInformation("\n--------------------------------------------\n\n");
            var address = addresses.Addresses.First();


            // Register service with consul
            var uri = new UriBuilder(address).Uri;
            var addr = $"{uri.Scheme}://{uri.Host}";

            var regId = $"{api.Key}-{GuidUtils.NewCleanGuid}";

            var registration = new AgentServiceRegistration
            {
                Id = regId,
                Name = $"{api.DisplayName} (v{api.Version})",
                Address = addr,
                Port = uri.Port,
                Tags = api.Tags.ToArray()
            };
            logger.LogInformation(
                $"Registering [{registration.Id}] with Consul @ [{ConsulConfig.Address}] in DC [{ConsulConfig.Dc}]");
            await consulClient.Agent.ServiceDeregister(registration.Id);
            await consulClient.Agent.ServiceRegister(registration);

            lifetime.ApplicationStopping.Register(async () =>
            {
                logger.LogInformation(
                    $"Deregistering [{registration.Id}] from Consul @ [{ConsulConfig.Address}]");
                await consulClient.Agent.ServiceDeregister(registration.Id);
            });


            return app;
        }
    }
}