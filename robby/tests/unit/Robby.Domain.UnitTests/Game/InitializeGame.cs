using System;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Robby.Domain.Game;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Domain.UnitTests.Game
{
    public static class InitializeGame
    {
        public class Tests : Aggregate.GameFeatureTests
        {
            private readonly string _name = "RobbyGame";

            public Tests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }


            [Fact]
            public void Must_HaveName()
            {
                Assert.False(string.IsNullOrWhiteSpace(_name));
            }

            [Fact]
            public void Must_BeInitialized()
            {
                Given(r => r.Model.Status == Schema.Game.Flags.Unknown);
                When(r => r.Execute(Cmd));
                Then(r => r.Model.Status == Schema.Game.Flags.Initialized);
            }


            protected override void SetTestEnvironment()
            {
            }

            protected override void InjectDependencies(IServiceCollection services)
            {
                services.AddGameActors();
            }

            protected override Schema.Game.ID CreateId()
            {
                throw new NotImplementedException();
            }

            protected override Domain.Game.Aggregate.Root CreateAggregate()
            {
                throw new NotImplementedException();
            }

            protected override Schema.Game CreateModel()
            {
                throw new NotImplementedException();
            }

            protected override Contract.Game.Features.Initialize.Fact CreateFact()
            {
                throw new NotImplementedException();
            }

            protected override Contract.Game.Features.Initialize.Feedback CreateFeedBack()
            {
                throw new NotImplementedException();
            }

            protected override Contract.Game.Features.Initialize.Hope CreateHope()
            {
                throw new NotImplementedException();
            }

            protected override Domain.Game.Initialize.Evt CreateEvt()
            {
                throw new NotImplementedException();
            }

            protected override Domain.Game.Initialize.Cmd CreateCmd()
            {
                throw new NotImplementedException();
            }
        }
    }
}