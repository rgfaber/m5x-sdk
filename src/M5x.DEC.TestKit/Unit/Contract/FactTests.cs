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

namespace M5x.DEC.TestKit.Unit.Contract;

public abstract class FactTests<TFact> : IoCTestsBase
    where TFact : IFact
{
    private string _attributedFactTopic;
    protected string ExpectedFactTopic;
    protected IFact Fact;

    protected FactTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public Task Needs_Fact()
    {
        try
        {
            Assert.NotNull(Fact);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Should_FactShouldBeOfTypeTFact()
    {
        try
        {
            Assert.IsType<TFact>(Fact);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Should_FactMustHaveTopic()
    {
        try
        {
            _attributedFactTopic = AttributeUtils.GetTopic<TFact>();
            Assert.False(string.IsNullOrWhiteSpace(_attributedFactTopic));
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_HaveExpectedFactTopic()
    {
        try
        {
            _attributedFactTopic = AttributeUtils.GetTopic<TFact>();
            ExpectedFactTopic = GetExpectedFactTopic();
            Assert.Equal(ExpectedFactTopic, _attributedFactTopic);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    protected abstract string GetExpectedFactTopic();


    [Fact]
    public void Must_FactMustBeDeserializable()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFact)Fact);
            var f = JsonSerializer.Deserialize<TFact>(sb);
            Assert.NotNull(f);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }
    }

    [Fact]
    public void Must_DeserializedFactMustBeTFact()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFact)Fact);
            var f = JsonSerializer.Deserialize<TFact>(sb);
            Assert.IsType<TFact>(f);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }
    }

    [Fact]
    public void Must_OriginalFactMustBeEqualToDeserializedFact()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFact)Fact);
            var fd = JsonSerializer.Deserialize<TFact>(sb);
            fd.ShouldBeEquivalentTo((TFact)Fact);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }
    }
}