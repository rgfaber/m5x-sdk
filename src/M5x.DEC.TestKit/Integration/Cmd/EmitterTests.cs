using System;
using System.DirectoryServices.ActiveDirectory;
using System.Threading;
using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class EmitterTests<TEmitter, TSubscriber, TAggregateId, TFact> : IoCTestsBase
        where TEmitter : IFactEmitter<TAggregateId, TFact>
        where TSubscriber : IHostedService
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        // internal class InitializeHandler : IFactHandler<TAggregateId, TFact>
        // {
        //     public Task HandleAsync(TFact fact)
        //     {
        //         _inFact = fact;
        //         Assert.True(_outFact.CorrelationId == _inFact.CorrelationId);
        //         return Task.CompletedTask;
        //     }
        // }

        
        
        protected TFact _inFact;

        protected string _correlationId; 
        
        protected TFact _outFact;

        private readonly int _maxRetries = Polly.Config.MaxRetries;

        private readonly AsyncRetryPolicy RetryPolicy;
//        private static Contract.Features.Initialize.Hope _someHope = Contract.Features.Initialize.Hope.New(_someId.Value, _correlationId, _someWorkOrder);

        protected TEmitter _emitter;
        protected TSubscriber _subscriber;

        protected IFactHandler<TAggregateId, TFact> _handler;
        private ILogger _logger;

        public EmitterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
            RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_maxRetries,
                    times => TimeSpan.FromMilliseconds(times * 100));
        }

        [Fact]
        public void Needs_Emitter()
        {
            Assert.NotNull(_emitter);
        }

        [Fact]
        public void Needs_Subscriber()
        {
            Assert.NotNull(_subscriber);
        }

        [Fact]
        public void Needs_Logger()
        {
            Assert.NotNull(_logger);
        }

        [Fact]
        public void Needs_Handler()
        {
            Assert.NotNull(_handler);
        }


        [Fact]
        public void Must_EmitterMustEmitFact()
        {
            var subHost = new HostExecutor(_logger, new IHostedService[] { _subscriber });
            try
            {
                subHost.StartAsync(CancellationToken.None).Wait();
                Output?.WriteLine($"Emitting Fact: {_outFact}");
                _emitter.EmitAsync(_outFact).Wait();
            }
            catch (Exception e)
            {
                Output?.WriteLine(e.InnerAndOuter());
                throw new AggregateException("Retrying Test", e);
            }
            finally
            {
                subHost.StopAsync(CancellationToken.None).Wait();
            }
        }
       
    }
}