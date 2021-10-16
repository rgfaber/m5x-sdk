using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl
{
    public abstract class WriterTests<TWriter, TReader, TAggregateId, TFact, TReadModel, TQuery> : IoCTestsBase
        where TWriter : IModelWriter<TAggregateId, TFact, TReadModel>
        where TReader: IModelReader<TQuery, TReadModel>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TReadModel : IReadEntity
        where TQuery : IQuery
    
    {
        protected TReader Reader;
        protected TWriter Writer;
        protected TQuery Query;
        protected TFact Fact;


        protected WriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Fact()
        {
            Assert.NotNull(Fact);
        }

        [Fact]
        public void Needs_Query()
        {
            Assert.NotNull(Query);
        }


        [Fact]
        public void Needs_Writer()
        {
            Assert.NotNull(Writer);
        }

        [Fact]
        public async Task Must_WriteFactToDb()
        {
            var res = await Writer.UpdateAsync((TFact)Fact);
            Assert.NotNull(res);
            Assert.IsType<TReadModel>(res);
            Assert.Equal(Fact.Meta.Id, res.Id);
        }

        [Fact]
        public async Task Must_ExistInDb()
        {
            await Must_WriteFactToDb();
            var rsp = await Reader.GetByIdAsync(Fact.Meta.Id);
            Assert.NotNull(rsp);
            Assert.IsType<TReadModel>(rsp);
            Assert.Equal(Fact.Meta.Id, rsp.Id);
        }

        [Fact]
        public async Task Must_DeleteFromDb()
        {
            await Must_ExistInDb();
            var delRsp = await Writer.DeleteAsync(Fact.Meta.Id);
            Assert.NotNull(delRsp);
            Assert.Equal(Fact.Meta.Id, delRsp.Id);
            Assert.True(string.IsNullOrWhiteSpace(delRsp.Prev));
            var deleted = await Reader.GetByIdAsync(Fact.Meta.Id);
            Assert.Null(deleted);
        }
        
        [Fact]
        public void Needs_Reader()
        {
            Assert.NotNull(Reader);
        }
    }
}