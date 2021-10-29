using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl
{
    public abstract class EventWriterTests<TWriter, TReader, TID, TEvent, TReadModel, TQuery> : IoCTestsBase
        where TWriter : IEventWriter<TID, TEvent, TReadModel>
        where TReader : ISingleModelReader<TQuery, TReadModel>
        where TID : IIdentity
        where TReadModel : IReadEntity
        where TQuery : IQuery
        where TEvent : IEvent<TID>

    {
        protected IEvent<TID> Event;
        protected IQuery Query;
        protected ISingleModelReader<TQuery,TReadModel> Reader;
        protected IEventWriter<TID, TEvent, TReadModel> Writer;
        protected IReadEntity Model;
        
        protected EventWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Reader()
        {
            Assert.NotNull(Reader);
        }

        [Fact]
        public void Must_ReaderMustBeAssignableFromTReader()
        {
            Assert.IsAssignableFrom<TReader>(Reader);
        }

        [Fact]
        public void Needs_Event()
        {
            Assert.NotNull(Event);
        }

        [Fact]
        public void Must_EventMustBeAssignableFromTEvent()
        {
            Assert.IsAssignableFrom<TEvent>(Event);
        }
        
        [Fact]
        public void Needs_Query()
        {
            Assert.NotNull(Query);
        }

        [Fact]
        public void Must_QueryMustBeAssignableFromTQuery()
        {
            Assert.IsAssignableFrom<TQuery>(Query);
        }
        


        [Fact]
        public void Needs_Writer()
        {
            Assert.NotNull(Writer);
        }

        [Fact]
        public void Must_WriterMustBeAssignableFromTWriter()
        {
            Assert.IsAssignableFrom<TWriter>(Writer);
        }
        

        [Fact]
        public async Task Must_WriteFactToDb()
        {
            var res = await Writer.UpdateAsync((TEvent)Event);
            Assert.NotNull(res);
            Assert.IsType<TReadModel>(res);
            Assert.Equal(Event.Meta.Id, res.Id);
        }

        [Fact]
        public async Task Must_ExistInDb()
        {
            await Must_WriteFactToDb();
            var rsp = await Reader.GetByIdAsync(Event.Meta.Id);
            Assert.NotNull(rsp);
            Assert.IsType<TReadModel>(rsp);
            Assert.Equal(Event.Meta.Id, rsp.Id);
        }

        [Fact]
        public async Task Must_DeleteFromDb()
        {
            await Must_ExistInDb();
            var delRsp = await Writer.DeleteAsync(Event.Meta.Id);
            Assert.NotNull(delRsp);
            Assert.Equal(Event.Meta.Id, delRsp.Id);
            Assert.True(string.IsNullOrWhiteSpace(delRsp.Prev));
            var deleted = await Reader.GetByIdAsync(Event.Meta.Id);
            Assert.Null(deleted);
        }

        
        
        
    }
}