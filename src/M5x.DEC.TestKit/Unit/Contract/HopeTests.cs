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

public abstract class HopeTests<THope, TFeedback> : IoCTestsBase
    where THope : IHope
    where TFeedback : IFeedback
{
    private string _attributedHopeTopic;
    protected string ExpectedHopeTopic;
    protected IFeedback Feedback;
    protected IHope Hope;

    public HopeTests(ITestOutputHelper output,
        IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public Task Needs_Hope()
    {
        try
        {
            Assert.NotNull(Hope);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }


    [Fact]
    public Task Should_HopeMustBeOfTypeTHope()
    {
        try
        {
            Assert.IsType<THope>(Hope);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }


    [Fact]
    public Task Needs_Feedback()
    {
        try
        {
            Assert.NotNull(Feedback);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Should_FeedBackMustBeOfTypeTFeedback()
    {
        try
        {
            Assert.IsType<TFeedback>(Feedback);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_HopeMustHaveTopic()
    {
        try
        {
            _attributedHopeTopic = AttributeUtils.GetTopic<THope>();
            Assert.False(string.IsNullOrWhiteSpace(_attributedHopeTopic));
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_HaveCorrectTopic()
    {
        try
        {
            _attributedHopeTopic = AttributeUtils.GetTopic<THope>();
            ExpectedHopeTopic = GetExpectedHopeTopic();
            Assert.Equal(ExpectedHopeTopic, _attributedHopeTopic);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    protected abstract string GetExpectedHopeTopic();


    [Fact]
    public Task Must_HopeMustBeDeserializable()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((THope)Hope);
            var hd = JsonSerializer.Deserialize<THope>(sb);
            Assert.NotNull(hd);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }


    [Fact]
    public Task Must_DeserializedHopeMustBeTHope()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((THope)Hope);
            var hd = JsonSerializer.Deserialize<THope>(sb);
            Assert.IsType<THope>(hd);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_OriginalHopeMustBeEqualToDeserializedHope()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((THope)Hope);
            var hd = JsonSerializer.Deserialize<THope>(sb);
            hd.ShouldBeEquivalentTo(Hope);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_FeedbackMustBeDeserializable()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFeedback)Feedback);
            var fbd = JsonSerializer.Deserialize<TFeedback>(sb);
            Assert.NotNull(fbd);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_DeserializedFeedbackMustBeOfTypeTFeedback()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFeedback)Feedback);
            var fbd = JsonSerializer.Deserialize<TFeedback>(sb);
            Assert.IsType<TFeedback>(fbd);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }


    [Fact]
    public Task Must_OriginalFeedbackMustBeEqualToDeserializedFeedback()
    {
        try
        {
            var sb = JsonSerializer.SerializeToUtf8Bytes((TFeedback)Feedback);
            var fbd = JsonSerializer.Deserialize<TFeedback>(sb);
            fbd.ShouldBeEquivalentTo(Feedback);
        }
        catch (Exception e)
        {
            Output.WriteLine(e.InnerAndOuter());
            throw;
        }

        return Task.CompletedTask;
    }
}