using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.Publisher.Contract;
using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace M5x.Stan.Subscriber.Cli
{
    internal class TrivialResponder : BackgroundService, ITrivialResponder
    {
        private readonly IEncodedConnection _conn;

        public TrivialResponder(IEncodedConnection conn)
        {
            _conn = conn;
            _conn.OnDeserialize = data =>
            {
                return JsonSerializer.Deserialize<TestRafReq>(data);
            };
            _conn.OnSerialize = o =>
            {
                return JsonSerializer.SerializeToUtf8Bytes(o);
            };
        }

        public override async Task StopAsync(CancellationToken token)
        {
            await _conn.DrainAsync();
            _conn.Close();
        }

        public override async Task StartAsync(CancellationToken token)
        {
            const string topic = "OnsHuis";
            Console.WriteLine($"Listening on [{topic}] via [{_conn.ConnectedUrl}]");
            _conn.SubscribeAsync(topic, (sender, args) =>
            {
                var req = args.ReceivedObject as TestRafReq;
                Console.WriteLine($"RECEIVED: TestRafReq {req.CorrelationId}");
                var rsp = new TestRafRsp(req.CorrelationId);
                if (!string.IsNullOrEmpty(args.Reply))
                {
                    _conn.Publish("OnsHuis", args.Reply, rsp);
                    Console.WriteLine($"Replied to {args.Reply}");
                }
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
            };
        }
    }

    public interface ITrivialResponder
    {
    }
}