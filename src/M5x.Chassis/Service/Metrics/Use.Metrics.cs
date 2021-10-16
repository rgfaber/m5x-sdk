using System.Text;
using M5x.Chassis.Container.Interfaces;
using M5x.Chassis.Mh;
using M5x.Chassis.Mh.Reporting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Chassis.Service.Metrics
{
    public partial class Use
    {
        private static IApplicationBuilder UseMetricsAndHealthChecks(this IApplicationBuilder app)
        {
            app.InstallMetrics()
                .Map("/metrics", HandleMetricsSampling)
                .Map("/health", HandleHealthChecksSampling);
            return app;
        }

        private static void HandleHealthChecksSampling(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var c = context.RequestServices.GetRequiredService<IContainer>();
                var h = c.Resolve<HealthChecks>("H");
                h.RunHealthChecks();
                var f = new MhReportFormatter(h);
                var j = f.GetSample();

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(j, Encoding.UTF8);
            });
        }

        private static void HandleMetricsSampling(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var c = context.RequestServices.GetRequiredService<IContainer>();
                var m = c.Resolve<Mh.Metrics>("M");
                var f = new MhReportFormatter(m);
                var j = f.GetSample();

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(j, Encoding.UTF8);
            });
        }

        private static IApplicationBuilder InstallMetrics(this IApplicationBuilder app)
        {
            var c = app.ApplicationServices.GetRequiredService<IContainer>();
            var m = c.Resolve<Mh.Metrics>("M");
            var reqps = m.Meter(typeof(Use), "requests_per_second", "requests", TimeUnit.Seconds);
            var resps = m.Meter(typeof(Use), "responses_per_second", "responses", TimeUnit.Seconds);

            return app.Use(async (context, next) =>
            {
                reqps.Mark();
                await next.Invoke();
                resps.Mark();
            });
        }
    }
}