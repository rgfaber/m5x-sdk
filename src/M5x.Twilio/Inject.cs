using Microsoft.Extensions.DependencyInjection;
using Twilio.Clients;
using Twilio.Http;

namespace M5x.Twilio;

public static class Inject
{
    public static IServiceCollection AddTwilio(this IServiceCollection services)
    {
        return services?
            .AddSingleton<ITwilioFactory, TwilioFactory>();
    }
}

internal class TwilioFactory : ITwilioFactory
{
    public ITwilioRestClient CreateTwilioClient()
    {
        return new TwilioRestClient(Config.UserName, Config.Password, Config.AccountSid, Config.Region, null,
            Config.Edge);
    }

    public ITwilioRestClient CreateTwilioClient(string userName, string passWord, string accountSid = null,
        string region = null, HttpClient httpClient = null, string edge = null)
    {
        return new TwilioRestClient(userName, passWord, accountSid, region, httpClient, edge);
    }
}

public interface ITwilioFactory
{
    ITwilioRestClient CreateTwilioClient();

    ITwilioRestClient CreateTwilioClient(string userName, string passWord, string accountSid = null,
        string region = null, HttpClient httpClient = null, string edge = null);
}