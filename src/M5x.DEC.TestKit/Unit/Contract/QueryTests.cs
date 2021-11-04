using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Contract
{
    public abstract class QueryTests<TQuery, TResponse> : IoCTestsBase
        where TQuery : IQuery
        where TResponse : IResponse
    {
        protected string CorrelationId;
        protected IQuery Qry;
        protected IResponse Rsp;


        public QueryTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
            CorrelationId = GuidUtils.NewCleanGuid;
        }


        [Fact]
        public Task Must_HaveQry()
        {
            try
            {
                Assert.NotNull(Qry);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_QueryMustBeOfTypeTQuery()
        {
            try
            {
                Assert.IsType<TQuery>(Qry);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_ResponseMustBeOfTypeTResponse()
        {
            try
            {
                Assert.IsType<TResponse>(Rsp);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_QueryMustBeDeserializable()
        {
            try
            {
                var sb = JsonSerializer.SerializeToUtf8Bytes((TQuery)Qry);
                var qd = JsonSerializer.Deserialize<TQuery>(sb);
                Assert.NotNull(qd);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_DeserializedQueryMustBeOfTypeTQuery()
        {
            try
            {
                var sb = JsonSerializer.SerializeToUtf8Bytes((TQuery)Qry);
                var qd = JsonSerializer.Deserialize<TQuery>(sb);
                Assert.IsType<TQuery>(qd);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_HaveRsp()
        {
            try
            {
                Assert.NotNull(Rsp);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_RspMustBeOfTypeTResponse()
        {
            try
            {
                Assert.IsType<TResponse>(Rsp);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_ResponseMustBeDeserializable()
        {
            try
            {
                var sb = JsonSerializer.SerializeToUtf8Bytes((TResponse)Rsp);
                var rsd = JsonSerializer.Deserialize<TResponse>(sb);
                Assert.NotNull(rsd);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_DeserializedResponseMustBeOfTypeTResponse()
        {
            try
            {
                var sb = JsonSerializer.SerializeToUtf8Bytes((TResponse)Rsp);
                var rsd = JsonSerializer.Deserialize<TResponse>(sb);
                Assert.IsType<TResponse>(rsd);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_OriginalResponseMustBeEqualToDeserializedResponse()
        {
            try
            {
                var sb = JsonSerializer.SerializeToUtf8Bytes((TResponse)Rsp);
                var rsd = JsonSerializer.Deserialize<TResponse>(sb);
                rsd.ShouldBeEquivalentTo(Rsp);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }
    }
}