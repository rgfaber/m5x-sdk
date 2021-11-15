using System;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Qry
{
    public abstract class ReaderTests<TReader, TQuery, TReadModel> : IoCTestsBase
        where TReader : IModelReader
        where TQuery : IQuery
        where TReadModel : IPayload
    {
        protected IQuery Query;
        protected TReader Reader;

        protected ReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public Task Must_QueryMustBeOfTypeTQuery()
        {
            try
            {
                Assert.IsAssignableFrom<TQuery>(Query);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }

            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Reader()
        {
            try
            {
                Assert.NotNull(Reader);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }

            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Query()
        {
            try
            {
                Assert.NotNull(Query);
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