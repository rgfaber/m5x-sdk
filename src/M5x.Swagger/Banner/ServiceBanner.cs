using System;
using Figgle;
using M5x.AsciiArt;
using Serilog;

namespace M5x.Swagger.Banner;

public interface IServiceBanner
{
    string Print(string banner);
    string Print(IApiSettings api);
    void SetTheme(BannerTheme theme);
}

internal class ServiceBanner : IServiceBanner
{
    public ServiceBanner(IBannerTheme theme = null)
    {
        if (theme == null)
            theme = BannerTheme.Default;
        _current = theme;
    }

    protected IBannerTheme _current { get; private set; } = BannerTheme.Default;

    public string Print(string banner)
    {
        Console.ForegroundColor = _current.Foreground;
        Console.BackgroundColor = _current.BackGround;
        var res = FiggleFonts.Small.Render(banner);
        Log.Information(res);
        return $"{res}\n";
    }

    public string Print(IApiSettings api)
    {
        var res = "ApiSettings are undefined";
        if (api != null)
        {
            res = Print("====================");
            res = res + Print($"Service: {api.DisplayName}");
            res = res + Print($"Version: {api.Version}");
            res = res + Print($"Tenant : {api.TenantKey}");
            res = res + Print($"SLA    : {api.TermsOfService}");
            res = res + Print("====================");
        }

        return res;
    }

    public void SetTheme(BannerTheme theme)
    {
        _current = theme;
    }
}