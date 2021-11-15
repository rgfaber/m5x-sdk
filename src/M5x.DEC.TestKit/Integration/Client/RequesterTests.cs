using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Client
{
    public abstract class
        RequesterTests<TConnection, TResponder, TRequester, TID, THope, TFeedback> : ConnectedTests<TConnection>
        where TResponder : IResponder
        where TID : IIdentity
        where THope : IHope
        where TFeedback : IFeedback
        where TRequester : IRequester<THope, TFeedback>
    {
        protected IFeedback Feedback;
        protected IHope Hope;
        protected IIdentity Id;
        protected ILogger Logger;
        protected IRequester<THope, TFeedback> Requester;
        protected IResponder Responder;


        public RequesterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public Task Needs_Feedback()
        {
            Assert.NotNull(Feedback);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_FeedbackMustBeTypeTFeedback()
        {
            Assert.IsType<TFeedback>(Feedback);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_ID()
        {
            Assert.NotNull(Id);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_IDMustBeTypeTID()
        {
            Assert.IsType<TID>(Id);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Responder()
        {
            Assert.NotNull(Responder);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Requester()
        {
            Assert.NotNull(Requester);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Logger()
        {
            Assert.NotNull(Logger);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_ResponderMustBeAssignableFromTResponder()
        {
            Assert.IsAssignableFrom<TResponder>(Responder);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_RequesterMustBeAssignableFromTRequester()
        {
            Assert.IsAssignableFrom<TRequester>(Requester);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Hope()
        {
            Assert.NotNull(Hope);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_HopeMustBeOfTypeTHope()
        {
            Assert.IsType<THope>(Hope);
            return Task.CompletedTask;
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
    }
}