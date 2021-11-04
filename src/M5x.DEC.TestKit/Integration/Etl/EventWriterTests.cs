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
        public Task Needs_Reader()
        {
            Assert.NotNull(Reader);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_ReaderMustBeAssignableFromTReader()
        {
            Assert.IsAssignableFrom<TReader>(Reader);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Event()
        {
            Assert.NotNull(Event);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_EventMustBeAssignableFromTEvent()
        {
            Assert.IsAssignableFrom<TEvent>(Event);
            return Task.CompletedTask;
        }
        
        [Fact]
        public Task Needs_Query()
        {
            Assert.NotNull(Query);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_QueryMustBeAssignableFromTQuery()
        {
            Assert.IsAssignableFrom<TQuery>(Query);
            return Task.CompletedTask;
        }
        


        [Fact]
        public Task Needs_Writer()
        {
            Assert.NotNull(Writer);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_WriterMustBeAssignableFromTWriter()
        {
            Assert.IsAssignableFrom<TWriter>(Writer);
            return Task.CompletedTask;
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
            Assert.True(!string.IsNullOrWhiteSpace(delRsp.Prev));
            var deleted = await Reader.GetByIdAsync(Event.Meta.Id);
            Assert.True( string.IsNullOrWhiteSpace(deleted.Id));
        }
        
    }
}