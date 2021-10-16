using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Infra.STAN;
using M5x.DEC.Persistence;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Polly;
using Polly.Retry;
using Serilog;

namespace Robby.Etl.Infra.Game
{
    public static partial class Inject
    {
        public static IServiceCollection AddInitializeEtl(this IServiceCollection services)
        {
            return services?
                .AddHostedService<Initialize.Subscriber>()
                .AddTransient<Initialize.IWriter, Initialize.Writer>();
        }
    }

    public static class Initialize
    {
        public class Subscriber : STANSubscriber<Schema.Game.ID, Contract.Game.Features.Initialize.Fact>
        {
            public Subscriber(IEncodedConnection conn,
                IEnumerable<IWriter> handlers, ILogger logger) : base(conn, handlers, logger)
            {
            }
        }


        public interface IWriter : IModelWriter<Schema.Game.ID, Contract.Game.Features.Initialize.Fact, Schema.Game>
        {
        }

        internal class Writer : CouchWriter<Schema.Game.ID, Contract.Game.Features.Initialize.Fact, Schema.Game>,
            IWriter
        {
            
            private const int MaxRetries = 100;

            private readonly AsyncRetryPolicy _retryPolicy;
            
            
            public Writer(IGameDb store, ILogger logger) : base(store, logger)
            {
                _retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));

            }

            public override async Task<Schema.Game> UpdateAsync(Contract.Game.Features.Initialize.Fact fact)
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    Log.Debug("Attempting Game Initialization...");
                    var res = Schema.Game.New(fact.Meta.Id, fact.Payload);
                    res.Description.Name = fact.Payload.Name;
                    res.Meta = fact.Meta;
                    res = Store.AddOrUpdateAsync(res).Result;
                    Log.Debug($"Game {res.Description.Name} - Succusfully initialized ");
                    return res;
                });
            }
        }
    }
}