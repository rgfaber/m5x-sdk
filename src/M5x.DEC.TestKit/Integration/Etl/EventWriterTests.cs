using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Infra;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl;

public abstract class EventWriterTests<
    TID,
    TWriter,
    TInterpreter,
    TReader,
    TEvent,
    TReadModel,
    TQuery> : IoCTestsBase
    where TWriter : IEtlWriter<TID, TEvent, TReadModel>
    where TReader : ISingleModelReader<TQuery, TReadModel>
    where TReadModel : IReadEntity
    where TQuery : IQuery
    where TEvent : IEvent<TID>
    where TInterpreter : IInterpreter<TReadModel, TEvent>
    where TID : IIdentity
{
    protected IEvent<IIdentity> Event;
    protected IReadEntity Model;
    protected IQuery Query;
    protected ISingleModelReader<TQuery, TReadModel> Reader;
    protected IEtlWriter<TID, TEvent, TReadModel> Writer;

    protected EventWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    public object Interpreter { get; set; }

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
        await Writer.HandleAsync((TEvent)Event);
        var res = await Reader.GetByIdAsync(Event.Meta.Id);
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
    public Task Needs_Interpreter()
    {
        Assert.NotNull(Interpreter);
        return Task.CompletedTask;
    }

    [Fact]
    public void Must_InterpreterMustBeAssignableToTInterpreter()
    {
        Interpreter.ShouldBeAssignableTo<TInterpreter>();
    }
}