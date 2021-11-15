using System;
using M5x.DEC.Schema;
using Microsoft.Extensions.Hosting;

namespace M5x.DEC
{
    public interface ISubscriber : IDisposable
    {
    }

    public interface ISubscriber<TAggregateId, TFact> : IHostedService, ISubscriber
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        public string Topic { get; }
    }
}