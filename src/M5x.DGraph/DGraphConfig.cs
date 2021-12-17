using System;
using Grpc.Core;

namespace M5x.DGraph;

public static class DGraphConfig
{
    public static string DGraphChannel =
        Environment.GetEnvironmentVariable(EnVars.DGRAPH_CHANNEL) ?? "dgraph.local:9080";


    public static string User =
        Environment.GetEnvironmentVariable(EnVars.DGRAPH_USER) ?? "";

    public static ChannelCredentials Credentials => GetChannelCredentials();

    private static ChannelCredentials GetChannelCredentials()
    {
        return ChannelCredentials.Insecure;
        // TODO: Figure out how this Security works , insecure for now
        // return ChannelCredentials.Create(ChannelCredentials.Insecure, 
        //     CallCredentials.FromInterceptor(new AsyncAuthInterceptor(
        //     (context, metadata) =>
        //     {
        //         
        //     })));
    }
}