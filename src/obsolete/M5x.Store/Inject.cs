using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using M5x.Store.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace M5x.Store
{
    public static class Inject
    {
        // public static IServiceCollection AddStoreBuilder(this IServiceCollection services)
        // {
        //     return services?
        //         .AddTransient<ICouchBuilder, CouchBuilder>()
        //         .AddTransient<IStoreBuilder, StoreBuilder>()
        //         .AddCompacter()
        //         .AddReplicator();
        // }

        public static IServiceCollection AddTransientCouchClient(this IServiceCollection services,
            Action<CouchOptionsBuilder> options)

        {
            return services?
                .AddTransient<ICouchClient, CouchClient>(x => new CouchClient(options));
        }

        // public static IApplicationBuilder UseStoreBuilder(this IApplicationBuilder app)
        // {
        //     Thread.Sleep(10000);
        //     var sb = app.ApplicationServices.GetRequiredService<IStoreBuilder>();
        //     if (sb == null) return app;
        //     try
        //     {
        //         var clt = sb.Build("probe");
        //         var res = clt.GetStoreInfo().Result;
        //         app.Map("/store", HandleStore);
        //         var info = JsonConvert.DeserializeObject<StoreInfo>(res);
        //         if (info != null)
        //             Log.Information($"*** {info.CouchDb} to CouchDb [v{info.Version}] by {info.Vendor.Name} ***");
        //         else
        //             throw new Exception("CouchDB cannot be reached");
        //         return app.UseReplicator(new List<string> {"probe"});
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Warning(e.Message);
        //         Log.Warning("Waiting 10s for data store to come online.");
        //         return app.UseStoreBuilder();
        //     }
        //     finally
        //     {
        //         Log.CloseAndFlush();
        //     }
        // }


        // public static IApplicationBuilder UseCompacter(this IApplicationBuilder app, IEnumerable<string> names)
        // {
        //     Thread.Sleep(10000);
        //     var lst = new List<string>(names);
        //     if (app == null) return null;
        //     var rep = app.ApplicationServices.GetRequiredService<ICompactDb>();
        //     foreach (var name in lst)
        //         try
        //         {
        //             Log.Information($"** Compacting [{name}]...");
        //             var res = rep.Compact(name).Result;
        //             if (res == null) continue;
        //             Log.Information($"\t{res.DbName} -> {res.IsSuccess}");
        //         }
        //         catch (Exception e)
        //         {
        //             Log.Information("Waiting 10s for data store to come online.");
        //             app.UseCompacter(lst);
        //             Log.Information(e, e.Message);
        //         }
        //         finally
        //         {
        //             Log.CloseAndFlush();
        //         }
        //
        //     return app;
        // }

        public static IApplicationBuilder UseReplicator(this IApplicationBuilder app, IEnumerable<string> names)
        {
            Thread.Sleep(10000);
            var lst = new List<string>(names);
            if (app == null) return null;
            var rep = app.ApplicationServices.GetRequiredService<IReplicateDb>();
            Task.Run(async () =>
            {
                foreach (var name in lst)
                    try
                    {
                        Log.Information($"** Replicating [{name}]...");
                        var res = await rep.Replicate(name);
                        if (res == null) continue;
                        foreach (var xResponse in res)
                            Log.Information($"\t{xResponse.Key} -> {xResponse.Value.IsSuccess}");
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Waiting 10s for data store to come online.");
                        Log.Warning(e.Message);
                        app.UseReplicator(lst);
                    }
                    finally
                    {
                        Log.CloseAndFlush();
                    }
            });
            return app;
        }


        // public static IServiceCollection AddReplicator(this IServiceCollection services)
        // {
        //     return services?
        //         .AddSingleton<IReplicateDb, DbReplicator>();
        // }
        //
        // public static IServiceCollection AddCompacter(this IServiceCollection services)
        // {
        //     return services?
        //         .AddSingleton<ICompactDb, DbCompacter>();
        // }

        // private static void HandleStore(IApplicationBuilder app)
        // {
        //     app.Run(async context =>
        //     {
        //         var sb = context.RequestServices.GetRequiredService<IStoreBuilder>();
        //         var clt = sb.Build("");
        //         context.Response.ContentType = "application/json";
        //         await context.Response.WriteAsync(await clt.GetStoreInfo(), Encoding.UTF8);
        //     });
        // }
    }
}