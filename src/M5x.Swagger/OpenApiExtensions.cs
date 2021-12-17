using Microsoft.OpenApi.Models;

namespace M5x.Swagger;

public static class OpenApiExtensions
{
    public static OpenApiInfo ToOpenApiInfo(this IApiSettings apiInfo)
    {
        if (apiInfo == null) return null;
        var res = new OpenApiInfo
        {
            Contact = apiInfo.Contact,
            Version = apiInfo.Version,
            Description = apiInfo.Description,
            License = apiInfo.License,
            TermsOfService = apiInfo.TermsOfService,
            Title = apiInfo.DisplayName
        };
        //if (apiInfo.Extensions == null) return res;
        //foreach (var apiInfoExtension in apiInfo.Extensions)
        //    res.Extensions.Add(apiInfoExtension.Key, new OpenApiExtension()
        //    {

        //    } );
        return res;
    }
}