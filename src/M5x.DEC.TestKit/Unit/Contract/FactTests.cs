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
    public abstract class FactTests<TFact>: IoCTestsBase
    where TFact : IFact
    {
        protected IFact Fact;
        protected string ExpectedFactTopic;
        private string _attributedFactTopic;
        
        protected FactTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Fact()
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
        }

        [Fact]
        public void Should_FactShouldBeOfTypeTFact()
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
        }

        [Fact]
        public void Should_FactMustHaveTopic()
        {
            try
            {
                _attributedFactTopic = GetAttributedFactTopic();
                Assert.False(string.IsNullOrWhiteSpace(_attributedFactTopic));
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }

        [Fact]
        public void Must_HaveExpectedFactTopic()
        {
            try
            {
                _attributedFactTopic = GetAttributedFactTopic();
                ExpectedFactTopic = GetExpectedFactTopic();
                Assert.Equal(ExpectedFactTopic, _attributedFactTopic);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
        }

        protected abstract string GetExpectedFactTopic();

        private static string GetAttributedFactTopic()
        {
            var atts = (TopicAttribute[])typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TFact)}!");
            return atts[0].Id;
        }


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
}