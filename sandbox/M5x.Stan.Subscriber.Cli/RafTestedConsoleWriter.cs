using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.Publisher.Contract;

namespace M5x.Stan.Subscriber.Cli
{
    public class RafTestedConsoleWriter : IRafTestedHandler
    {
        public Task HandleAsync(RafTested @event)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Received Event: {JsonSerializer.Serialize(@event)}");
            });
        }
    }
}