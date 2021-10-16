using System.Net;
using M5x.Minio.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace M5x.Minio
{
    public static class Inject
    {
        public static IServiceCollection AddS3Client(this IServiceCollection services,
            string endpoint,
            string publicKey,
            string privateKey,
            string region,
            string sessionToken,
            bool withSSL = false,
            WebProxy proxy = null)
        {
            return services?
                .AddTransient(p =>
                {
                    var c = new MinioClient(endpoint,
                        publicKey,
                        privateKey,
                        region,
                        sessionToken);
                    if (withSSL)
                        c.WithSSL();
                    if (proxy != null)
                        c.WithProxy(proxy);
                    return c;
                })
                .AddTransient<IS3Client, S3Client>();
        }
    }
}