using Xunit.Abstractions;

namespace M5x.Nats.Tests
{
    public class NATSMessageSink : IMessageSink
    {
        private readonly ITestOutputHelper _output;

        public NATSMessageSink(ITestOutputHelper output)
        {
            _output = output;
        }

        public bool OnMessage(IMessageSinkMessage message)
        {
            if (message is ITestPassed)
                _output.WriteLine("Execution time was an awesome " + ((ITestPassed)message).ExecutionTime);
            // Return `false` if you want to interrupt test execution.
            return true;
        }
    }
}