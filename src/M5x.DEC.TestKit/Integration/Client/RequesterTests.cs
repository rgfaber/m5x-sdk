using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.TestKit.Integration.Cmd;
using M5x.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Client
{
    public abstract class RequesterTests<TConnection,TResponder, TRequester, TID, THope, TFeedback> : ConnectedTests<TConnection>
        where TResponder : IResponder
        where TID : IIdentity
        where THope : IHope
        where TFeedback : IFeedback
        where TRequester: IRequester<THope,TFeedback>
    {
        protected ILogger Logger;
        protected IRequester<THope,TFeedback> Requester;
        protected IResponder Responder;
        protected IHope Hope;
        protected IIdentity Id;
        protected IFeedback Feedback;
        


        public RequesterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public void Needs_Feedback()
        {
            Assert.NotNull(Feedback);
        }

        [Fact]
        public void Must_FeedbackMustBeTypeTFeedback()
        {
            Assert.IsType<TFeedback>(Feedback);
        }
        
        
        [Fact]
        public void Needs_ID()
        {
            Assert.NotNull(Id);
        }

        [Fact]
        public void Must_IDMustBeTypeTID()
        {
            Assert.IsType<TID>(Id);
        }


        [Fact]
        public void Needs_Responder()
        {
            Assert.NotNull(Responder);
        }

        [Fact]
        public void Needs_Requester()
        {
            Assert.NotNull(Requester);
        }

        [Fact]
        public void Needs_Logger()
        {
            Assert.NotNull(Logger);
        }

        [Fact]
        public void Must_ResponderMustBeAssignableFromTResponder()
        {
            Assert.IsAssignableFrom<TResponder>(Responder);
        }

        [Fact]
        public void Must_RequesterMustBeAssignableFromTRequester()
        {
            Assert.IsAssignableFrom<TRequester>(Requester);
        }

        [Fact]
        public void Needs_Hope()
        {
            Assert.NotNull(Hope);
        }

        [Fact]
        public void Must_HopeMustBeOfTypeTHope()
        {
            Assert.IsType<THope>(Hope);
        }
        
        
        [Fact]
        public async Task Should_ResponderShouldRespondToHope()
        {
            var cs = new CancellationTokenSource();
            var _responder = Container.GetRequiredService<IHostExecutor>();
            Assert.NotNull(_responder);
            var hope = (THope)Hope;
            Assert.NotNull(hope);
            try
            {
                await _responder.StartAsync(cs.Token).ConfigureAwait(false);
                var fbk = await Requester.RequestAsync(hope, cs.Token).ConfigureAwait(false);
                Assert.NotNull(fbk);
                Assert.IsType<TFeedback>(fbk);
            }
            catch (Exception e)
            {
                Output.WriteLine(e.InnerAndOuter());
                throw;
            }
            finally
            {
                cs.Cancel();
                await _responder.StopAsync(cs.Token).ConfigureAwait(false);
            }
        }        
        
        
        // public class TheFactHandler : IHopHandler<TID, THope>
        // {
        //     public Task HandleAsync(TFact fact)
        //     {
        //         EmitterTests<,,,,>.TestFacts.InFact = fact;
        //         Assert.True(EmitterTests<,,,,>.TestFacts.OutFact.CorrelationId == EmitterTests<,,,,>.TestFacts.InFact.CorrelationId);
        //         return Task.CompletedTask;
        //     }
        // }
        //
        // public static class TestFacts
        // {
        //     public static TFact InFact;
        //     public static TFact OutFact;
        // }
        //
        // public static class TestEvents
        // {
        //     public static TEvent OutEvent;
        // }
        
        
 
    }
}