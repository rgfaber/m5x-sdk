using System;
using KuzzleSdk;
using KuzzleSdk.Protocol;
using M5x.Kuzzle.Interfaces;

namespace M5x.Kuzzle;

public class KuzzleBuilder : IKuzzleBuilder
{
    public IKuzzleApi BuildApi(string url = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            url = KuzzleConfig.Url;
        var socket = new WebSocket(new Uri(url));
        return new KuzzleSdk.Kuzzle(socket);
    }
}