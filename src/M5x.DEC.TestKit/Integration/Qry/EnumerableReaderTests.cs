using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Qry
{
    public abstract class EnumerableReaderTests<TReader, TQuery, TReadModel> : ReaderTests<TReader, TQuery, TReadModel>
        where TReader : IModelReader<TQuery, TReadModel>
        where TQuery : IQuery
        where TReadModel : IPayload
    {
        protected EnumerableReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public Task Must_QueryMustBeMultiQuery()
        {
            try
            {
                Assert.IsAssignableFrom<MultiQuery>(Query);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            return Task.CompletedTask;
        }


        [Fact]
        public async Task Must_FindAllMustReturnEnumerableReadModel()
        {
            try
            {
                var res = await Reader.FindAllAsync((TQuery)Query);
                Assert.NotNull(res);
                var test = res.ToList();
                Assert.IsType<List<TReadModel>>(test);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }
    }
}