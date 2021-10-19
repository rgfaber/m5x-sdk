using M5x.Testing;
using MassTransit;
using MassTransit.Clients;
using MassTransit.Definition;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.RabbitMQ.Tests
{
    public class RequestReplyTests: RabbitMQTestsBase
    {
        private TestResponder _responder;
        private RequestClient<TestRequest> _requester;

        public RequestReplyTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            _responder = Container.GetService<TestResponder>();
            _requester = Container.GetService<RequestClient<TestRequest>>();
        }

        protected override void SetTestEnvironment()
        {
            
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddRabbitMQ(x =>
                {
                    x.AddConsumer<TestResponder>(typeof(TestResponderDefinition));
                    x.AddRequestClient<TestRequest>();
                    x.SetKebabCaseEndpointNameFormatter();
                    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
                });
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
        
        
        
        
        
        
        
        
    }

    internal class TestRequest
    {
    }

    public class TestResponderDefinition : ConsumerDefinition<TestResponder>
    {
        public TestResponderDefinition()
        {
            EndpointName = TestConstants.TEST_HOPE_TOPIC;
        }
    }

    
    public class TestResponder : IConsumer
    {
    }
}