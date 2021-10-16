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
    public abstract class SingletonReaderTests<TReader, TQuery, TReadModel> : ReaderTests<TReader, TQuery, TReadModel>
        where TReader: ISingleModelReader<TQuery,TReadModel> 
        where TQuery : ISingletonQuery 
        where TReadModel : IPayload
    {
        protected SingletonReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        

        [Fact]
        public void Must_QueryMustBeSingletonQuery()
        {
            try
            {
                Assert.True(Query is SingletonQuery);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }

        
        [Fact]
        public async Task Must_GetByIdMustReturnSingletonReadModel()
        {
            try
            {
                if (Query is SingletonQuery)
                {
                    var id = ((TQuery)Query).Id;
                    var res = await Reader.GetByIdAsync(id);
                    Assert.NotNull(res);
                    Assert.IsType<TReadModel>((TReadModel)res);
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }
    }
}