using System;
using Dgraph;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DGraph
{
    public static class Inject
    {
        public static IServiceCollection AddDGraph(this IServiceCollection services)
        {
            return services?
                .AddTransient<IDgraphClient, DgraphClient>(x =>
                    new DgraphClient(GrpcChannel.ForAddress(
                        new Uri(DGraphConfig.DGraphChannel),
                        new GrpcChannelOptions
                        {
                            Credentials = ChannelCredentials.Insecure
                        })))
                .AddTransient<IGraphClient, GraphClient>();
        }
    }
}