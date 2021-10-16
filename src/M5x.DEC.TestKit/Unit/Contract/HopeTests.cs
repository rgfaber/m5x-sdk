using System;
using System.Text.Json;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Contract
{
    public abstract class HopeTests<THope, TFeedback> : IoCTestsBase
        where THope : IHope
        where TFeedback : IFeedback
    {
        protected string ExpectedHopeTopic;
        private string _attributedHopeTopic;
        protected IFeedback Feedback;
        protected IHope Hope;

        public HopeTests(ITestOutputHelper output,
            IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Hope()
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
        }


        [Fact]
        public void Should_HopeMustBeOfTypeTHope()
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
        }


        [Fact]
        public void Needs_Feedback()
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
        }

        [Fact]
        public void Should_FeedBackMustBeOfTypeTFeedback()
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
        }

        [Fact]
        public void Must_HopeMustHaveTopic()
        {
            try
            {
                _attributedHopeTopic = GetAttributedHopeTopic();
                Assert.False(string.IsNullOrWhiteSpace(_attributedHopeTopic));
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }

        [Fact]
        public void Must_HaveCorrectTopic()
        {
            try
            {
                _attributedHopeTopic = GetAttributedHopeTopic();
                ExpectedHopeTopic = GetExpectedHopeTopic();
                Assert.Equal(ExpectedHopeTopic, _attributedHopeTopic);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }

        protected abstract string GetExpectedHopeTopic();


        protected virtual string GetAttributedHopeTopic()
        {
            var attrs = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(THope)}!";
        }

        [Fact]
        public void Must_HopeMustBeDeserializable()
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
        } 
        
        
        [Fact]
        public void Must_DeserializedHopeMustBeTHope()
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
        }

        [Fact]
        public void Must_OriginalHopeMustBeEqualToDeserializedHope()
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
        }

        [Fact]
        public void Must_FeedbackMustBeDeserializable()
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
        }

        [Fact]
        public void Must_DeserializedFeedbackMustBeOfTypeTFeedback()
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
        }


        [Fact]
        public void Must_OriginalFeedbackMustBeEqualToDeserializedFeedback()
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
        }





    }
}