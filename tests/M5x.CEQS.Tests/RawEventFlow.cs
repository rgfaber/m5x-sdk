using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;
using EventFlow.DependencyInjection.Extensions;
using EventFlow.Extensions;
using EventFlow.ReadStores;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.Tests
{
    public class RawEventFlow: IoCTestsBase
    {
        private ICommandBus _bus;

        [Fact]
        public void Test1()
        {

        }

        public RawEventFlow(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override async void Initialize()
        {
            _bus = Container.GetService<ICommandBus>();
        }

        protected override void SetTestEnvironment()
        {
            
        }


        [Fact]
        public async Task Should_PublishCommand()
        {
            try
            {
                var cmd = new MyCommand(MyId.New);
                var res =await _bus.PublishAsync<MyAggregate,MyId,MyFeedBack>(cmd, CancellationToken.None);
                Assert.NotNull(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddEventFlow(o => o
                .AddCommands(new[]
                {
                    typeof(MyCommand)
                })
                .AddCommandHandlers(new []
                {
                    typeof(MyHandler)
                })
            
            );
        }
        
        
        
    }

    public class MyHandler: CommandHandler<MyAggregate, MyId, MyCommand>
    {
        public override Task ExecuteAsync(MyAggregate aggregate, MyCommand command, CancellationToken cancellationToken)
        {
            Log.Verbose("Hi There");
            return Task.CompletedTask;
        }
    }

    public class MyCommand: Command<MyAggregate, MyId, MyFeedBack>, ICommand<MyAggregate, MyId, IExecutionResult>
    {
        public MyCommand(MyId aggregateId) : base(aggregateId)
        {
        }

        public MyCommand(MyId aggregateId, ISourceId sourceId) : base(aggregateId, sourceId)
        {
        }
    }

    public class MyFeedBack : IExecutionResult
    {
        public bool IsSuccess { get; }
    }

    public class MyId : Identity<MyId>
    {
        public MyId(string value) : base(value)
        {
        }
    }

    public class MyAggregate: AggregateRoot<MyAggregate,MyId>
    {
        public MyAggregate(MyId id) : base(id)
        {
        }
    }
}
