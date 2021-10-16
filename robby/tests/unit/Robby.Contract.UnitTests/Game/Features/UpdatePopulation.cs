using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Infra;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Schema;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Contract.UnitTests.Game.Features
{
    public class UpdatePopulation : IoCTestsBase
    {
        private Population _population;

        public UpdatePopulation(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public async Task Should_FactMustBeDeserialized()
        {
            var gameId = Schema.Game.ID.New(Guid.NewGuid().ToString());
            var meta = AggregateInfo.New(gameId.Value, -1, (int)Schema.Game.Flags.Unknown);
            var fIn = Contract.Game.Features.UpdatePopulation.Fact.New(meta,
                GuidUtils.NewCleanGuid, _population);
            var s = JsonSerializer.SerializeToUtf8Bytes(fIn);
            var fOut = await JsonSerializer.DeserializeAsync<Contract.Game.Features.UpdatePopulation.Fact>(s.AsStream());
            Assert.Equal(fIn.Meta.Id, fOut.Meta.Id);
        }

        protected override void Initialize()
        {
            _population = Population.New(20, new Dimensions(20, 20, 20));
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }
    }
}