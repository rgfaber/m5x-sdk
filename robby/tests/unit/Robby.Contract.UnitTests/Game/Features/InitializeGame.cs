using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Infra;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Contract.Game.Features;
using Robby.Game.Contract.Features;
using Robby.Game.Schema;
using Robby.Schema;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Contract.UnitTests.Game.Features
{
    public static class InitializeGame
    {
        public class Tests : Aggregate.FeatureTests<Robby.Game.Contract.Features.Initialize.Fact,
            Robby.Game.Contract.Features.Initialize.Hope, Robby.Game.Contract.Features.Initialize.Fbk>
        {
            private InitializationOrder _order;

            public Tests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }

            [Fact]
            public void Must_HaveOrder()
            {
                Assert.NotNull(_order);
            }


            [Fact]
            public async Task Should_FactMustBeDeserialized()
            {
                var gameId = GameModel.ID.New.Value;
                var fIn = Robby.Game.Contract.Features.Initialize.Fact.New(
                    AggregateInfo.New(gameId), 
                    GuidUtils.NewCleanGuid, 
                    _order);
                var s = JsonSerializer.SerializeToUtf8Bytes(fIn);
                var fOut = await JsonSerializer
                    .DeserializeAsync<Robby.Game.Contract.Features.Initialize.Fact>(s.AsStream());
                Assert.Equal(fIn, fOut);
            }


            [Fact]
            public async Task Should_OrderMustBeDeserialized()
            {
                var s = JsonSerializer.SerializeToUtf8Bytes(_order);
                var oOut = await JsonSerializer.DeserializeAsync<InitializationOrder>(s.AsStream());
                Assert.Equal(_order, oOut);
            }


            protected override Robby.Game.Contract.Features.Initialize.Fbk CreateFeedback(
                Robby.Game.Schema.GameModel.ID gameId,
                string correlationId)
            {
                return Initialize.Fbk.Empty(correlationId);
            }

            protected override Contract.Game.Features.Initialize.Fact CreateFact(Schema.Game.ID gameId, string correlationId)
            {
                var meta = AggregateInfo.New(gameId.Value, -1, (int)Schema.Game.Flags.Unknown);
                return Contract.Game.Features.Initialize.Fact.New(meta, correlationId, _order);
            }

            protected override Contract.Game.Features.Initialize.Hope CreateHope(Schema.Game.ID gameId,
                string correlationId)
            {
                return Contract.Game.Features.Initialize.Hope.New(correlationId, _order);
            }

            protected override Schema.Game.ID CreateId()
            {
                return Identity<Schema.Game.ID>.New;
            }
        }
    }
}