using System.Threading.Tasks;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Utils;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Unit.Contract
{
        public abstract class QueryTests<TQuery, TResponse> : IoCTestsBase 
            where TQuery: IQuery 
            where TResponse: IResponse
        {
            protected IQuery Qry;
            protected IResponse Rsp;
            protected string CorrelationId;


            [Fact]
            public void Must_HaveQry()
            {
                Assert.NotNull(Qry);
            }

            [Fact]
            public void Must_HaveRsp()
            {
                Assert.NotNull(Rsp);
            }

            [Fact]
            public void Must_HaveCorrelationId()
            {
                Assert.False(string.IsNullOrWhiteSpace(CorrelationId));
            }
            
            
            public QueryTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
                CorrelationId = GuidUtils.NewCleanGuid;
            }

            protected override async void Initialize()
            {
                Qry = CreateQuery(CorrelationId);
                Rsp = CreateResponse(CorrelationId);
            }
            
            protected abstract TResponse CreateResponse(string correlationId);
            protected abstract TQuery CreateQuery(string correlationId);
        }
}