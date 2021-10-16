using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Robby.Contract.UnitTests.Game.Queries
{
    public static class First20
    {
        public class QueryTests : Aggregate.QueryTests<Contract.Game.Queries.First20.Qry, Contract.Game.Queries.First20.Rsp>
        {
            private Contract.Game.Queries.First20.Qry _qry;

            public QueryTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
            }

            protected override void SetTestEnvironment()
            {
            }

            protected override void InjectDependencies(IServiceCollection services)
            {
            }

            protected override Contract.Game.Queries.First20.Rsp CreateResponse(string correlationId)
            {
                return Contract.Game.Queries.First20.Rsp.New(_qry);
            }

            protected override Contract.Game.Queries.First20.Qry CreateQuery(string correlationId)
            {
                return Contract.Game.Queries.First20.Qry.New(correlationId);
            }
        }
    }
}