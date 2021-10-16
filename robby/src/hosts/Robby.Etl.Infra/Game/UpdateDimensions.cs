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
using Robby.Schema;
using Serilog;

namespace Robby.Etl.Infra.Game
{
    public static class UpdateDimensions
    {
        public static IServiceCollection AddUpdateDimensionsEtl(this IServiceCollection services)
        {
            return services?
                .AddTransient<IWriter, Writer>()
                .AddHostedService<Subscriber>();
        }


        internal class Subscriber : STANSubscriber<Schema.Game.ID, Contract.Game.Features.UpdateDimensions.Fact>
        {
            public Subscriber(IEncodedConnection conn,
                IEnumerable<IWriter> handlers,
                ILogger logger) : base(conn,
                handlers,
                logger)
            {
            }
        }

        public interface IWriter : IFactHandler<Schema.Game.ID, Contract.Game.Features.UpdateDimensions.Fact>
        {
        }

        internal class Writer : CouchWriter<Schema.Game.ID, Contract.Game.Features.UpdateDimensions.Fact,
            Schema.Game>, IWriter
        {
            private const int MaxRetries = 100;

            private readonly AsyncRetryPolicy _retryPolicy;


            public Writer(IGameDb store, ILogger logger) : base(store, logger)
            {
                _retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(MaxRetries, times => TimeSpan.FromMilliseconds(times * 100));
            }

            public override async Task<Schema.Game> UpdateAsync(Contract.Game.Features.UpdateDimensions.Fact fact)
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    Log.Debug("Attempting Update World Dimensions...");
                    var sim = await Store.GetByIdAsync(fact.Meta.Id);
                    if (sim == null)
                    {
                        Log.Warning($"Key [{fact.Meta.Id}] was not found, will retry.");
                        throw new KeyNotFoundException();
                    }
                    sim.Dimensions = fact.Payload;
                    sim.Meta = fact.Meta;
                    sim = Store.AddOrUpdateAsync(sim).Result;
                    Log.Debug($"{sim.Description.Name} - World Dimensions Succesfully Updated");
                    return sim;
                });
            }
        }
    }
}