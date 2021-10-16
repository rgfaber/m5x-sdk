using System;
using System.Text;
using System.Text.Json;
using M5x.AsciiArt;
using M5x.Swagger.Banner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace M5x.Swagger
{
    public static class Inject
    {
        public static IServiceCollection AddBanner(this IServiceCollection services, IBannerTheme theme = null)
        {
            return services?
                .AddBannerTheme()
                .AddSingleton<IServiceBanner, ServiceBanner>();
        }


        public static IApplicationBuilder UseBanner(this IApplicationBuilder app, IApiSettings api)
        {
            var ban = app.ApplicationServices.GetService<IServiceBanner>();
            //ban?.Print(api);
            Log.Information(JsonSerializer.Serialize(api));
            return app;
        }


        public static IApplicationBuilder UseBannerApi(this IApplicationBuilder app, IApiSettings api)
        {
            return app.Map("/banner",
                x =>
                {
                    x.Run(async ctx =>
                    {
                        var j = JsonSerializer.Serialize(api);
                        ctx.Response.ContentType = "application/json";
                        await ctx.Response.WriteAsync(j, Encoding.UTF8);
                    });
                });
        }


        public static IApplicationBuilder UseBanner(this IApplicationBuilder app, string banner)
        {
            var ban = app.ApplicationServices.GetService<IServiceBanner>();
            ban?.Print(banner);
            return app;
        }


        public static IServiceCollection AddBannerTheme(this IServiceCollection services, IBannerTheme theme = null)
        {
            if (theme == null)
                theme = BannerTheme.Default;
            return services?
                .AddSingleton(theme);
        }


        public static IServiceCollection AddApiDef(this IServiceCollection services,
            IApiSettings api)
        {
            return services?
                .AddSingleton(api);
        }


        public static IServiceCollection AddXwagger(this IServiceCollection services, IApiSettings apiInfo)
        {
            services?
                .AddMvcCore()
                .AddApiExplorer();
            services?
                .AddSwaggerGen(c =>
                {
                    try
                    {
                        // c.DescribeAllEnumsAsStrings();
                        if (!OpenApiConfig.DoNotUseFullTypeNames)
                            c.CustomSchemaIds(x => x.FullName);
                        if (!OpenApiConfig.DoNotFilterNamespaceSchema)
                            c.SchemaFilter<NamespaceSchemaFilter>();
                        c.SwaggerDoc("v2", apiInfo.ToOpenApiInfo());
                        c.IgnoreObsoleteActions();
                        c.IgnoreObsoleteProperties();
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(e, e.Message);
                    }
                });
            return services;
        }


        public static IApplicationBuilder UseXwagger(this IApplicationBuilder app,
            string title)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                try
                {
                    x.SwaggerEndpoint("/swagger/v2/swagger.json", title);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            });
            return app;
        }
    }
}