using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.Publisher.Contract;
using Microsoft.CodeAnalysis;
using NATS.Client;
using Org.BouncyCastle.Crypto.Tls;

namespace M5x.Stan.Publisher.Cli
{
    public class TrivialRequester : ITrivialRequester
    {
        private readonly IEncodedConnection _conn;

        public TrivialRequester(IEncodedConnection conn)
        {
            _conn = conn;
            _conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes(o);
            _conn.OnDeserialize = data => JsonSerializer.Deserialize<TestRafRsp>(data);
        }


        public async Task<TestRafRsp> RequestAsync(TestRafReq req)
        {
            var topic = "OnsHuis";
            Console.WriteLine($"Requesting [{req}] on [{topic}] via [{_conn.ConnectedUrl}]");
            var rsp = _conn.Request(topic, req) as TestRafRsp;
            Console.WriteLine($"Received response: {rsp}");
            return rsp;
        }
    }

    public interface ITrivialRequester
    {
        Task<TestRafRsp> RequestAsync(TestRafReq req);
    }
}