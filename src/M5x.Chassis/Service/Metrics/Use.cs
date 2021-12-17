using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace M5x.Chassis.Service.Metrics;

public static partial class Use
{
    private const string RoundTripHeader = "Mh-RoundTrip-Milliseconds";

    public static IApplicationBuilder UseMh(this IApplicationBuilder app)
    {
        app.UseMetricsAndHealthChecks();

        return app.Use(async (context, next) =>
        {
            var sw = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(RoundTripHeader,
                    sw.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

                return Task.CompletedTask;
            });

            await next.Invoke();
        });
    }
}