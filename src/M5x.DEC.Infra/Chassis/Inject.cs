using System.Threading;
using M5x.Chassis.Service.Metrics;
using M5x.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace M5x.DEC.Infra.Chassis
{
    public static class Inject
    {
        public static IServiceCollection AddBaseApi(this IServiceCollection services,
            IApiSettings apiInfo)
        {
            services.AddMvcCore()
                .AddApiExplorer();
            return services?
                .AddApiDef(apiInfo)
                .AddMh()
                .AddXwagger(apiInfo);
        }


        public static IApplicationBuilder UseBaseApi(this IApplicationBuilder app,
            IHostEnvironment env,
            string displayName,
            ILoggerFactory loggerFactory)
        {
            return app?
                .UseBaseApi(env, null, displayName, loggerFactory);
        }


        public static IApplicationBuilder UseBaseApi(this IApplicationBuilder app,
            IHostEnvironment env,
            IHostApplicationLifetime lifeTime,
            string displayName,
            ILoggerFactory loggerFactory)
        {
            Thread.Sleep(5000);
            // if (ApiConfig.IsReverse)
            //     app.UseForwardedHeaders(new ForwardedHeadersOptions
            //     {
            //         ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //     });
            app.UseStaticFiles();
            app.UseMh();
            app.UseXwagger(displayName);
            loggerFactory.AddSerilog();
            return app;
        }
    }
}