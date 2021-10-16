using Xunit;
using Xunit.Abstractions;

namespace M5x.Nats.Tests
{
    public class NATSRunnerReporter : IRunnerReporter
    {
        private readonly ITestOutputHelper _output;

        public NATSRunnerReporter(ITestOutputHelper output)
        {
            _output = output;
        }

        public string Description => "NATS runner reporter";
        public bool IsEnvironmentallyEnabled => true;
        public string RunnerSwitch => "natsrunnerreporter";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            return new NATSMessageSink(_output);
        }
    }
}