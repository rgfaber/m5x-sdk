using System;
using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Infra;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Polly;
using Polly.Retry;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class ResponderTests<TResponder,TRequester, TAggregate,TID,THope,TCmd,TFeedback> : IoCTestsBase
    where TResponder : IResponder<TAggregate, TID, THope, TCmd, TFeedback>
    where TAggregate : IAggregateRoot<TID>
    where TID : IIdentity
    where THope : IHope
    where TCmd : ICommand<TAggregate, TID, IExecutionResult>
    where TFeedback : IFeedback
    {
        protected static string _correlationId = GuidUtils.NewCleanGuid;
        private readonly int _maxRetries = Polly.Config.MaxRetries;

        protected readonly AsyncRetryPolicy RetryPolicy;
        protected ILogger _logger;
        protected TRequester _requester;
        protected TResponder _responder;



        public ResponderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
            RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_maxRetries,
                    times => TimeSpan.FromMilliseconds(times * 100));
        }


        [Fact]
        public void Needs_Responder()
        {
            Assert.NotNull(_responder);
        }

        [Fact]
        public void Needs_Requester()
        {
            Assert.NotNull(_requester);
        }

        [Fact]
        public void Needs_Logger()
        {
            Assert.NotNull(_logger);
        }
    }
}