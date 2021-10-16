using System;
using System.Threading.Tasks;
using MyNatsClient;
using MyNatsClient.Encodings.Json;
using MyNatsClient.Ops;

namespace M5x.Nats
{
    public static class Extensions
    {
        public static T NatsDecode<T>(this ReadOnlyMemory<byte> payload, IEncoding decoding = null)
        {
            decoding ??= JsonEncoding.Default;

            var decoded = (T)decoding.Decode(payload.Span, typeof(T));

            return decoded;
        }

        public static ReadOnlyMemory<byte> NatsEncode<T>(this T item, IEncoding encoding = null) where T : class
        {
            encoding ??= JsonEncoding.Default;

            var encoded = encoding.Encode(item);

            return encoded;
        }


        public static ISubscription Subscribe(this INatsClient bus, string subject, Func<MsgOp, Task> onNext,
            Action<Exception> onError)
        {
            return bus.Sub(subject, x => x.Subscribe(async msgOp =>
            {
                try
                {
                    await onNext(msgOp);
                }
                catch (Exception ex)
                {
                    onError(ex);
                }
            }));
        }

        public static ISubscription Subscribe(this INatsClient bus, string subject, Action<MsgOp> onNext,
            Action<Exception> onError)
        {
            return bus.Sub(subject, x => x.Subscribe(onNext, onError));
        }
    }
}