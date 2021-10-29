using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl
{
    public abstract class FactWriterTests<TWriter, TReader, TAggregateId, TFact, TReadModel, TQuery> : IoCTestsBase
        where TWriter : IFactWriter<TAggregateId, TFact, TReadModel>
        where TReader : ISingleModelReader<TQuery, TReadModel>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TReadModel : IReadEntity
        where TQuery : IQuery
    {
        protected TFact Fact;
        protected TQuery Query;
        protected TReader Reader;
        protected TWriter Writer;


        protected FactWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
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
            var res = await Writer.UpdateAsync(Fact);
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