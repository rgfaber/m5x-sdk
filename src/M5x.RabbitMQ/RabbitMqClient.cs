using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace M5x.RabbitMQ
{
    public interface IRabbitMqClient : IConnection
    {
    }


    internal class RabbitMqClient : IRabbitMqClient
    {
        private readonly ILogger _logger;


        private readonly IConnection _rabbitMqClientImplementation;

        public RabbitMqClient(IConnectionFactory connectionFactory, ILogger logger)
        {
            _logger = logger;
            _rabbitMqClientImplementation = connectionFactory.CreateConnection();
        }

        public int LocalPort => _rabbitMqClientImplementation.LocalPort;

        public int RemotePort => _rabbitMqClientImplementation.RemotePort;

        public void Dispose()
        {
            _rabbitMqClientImplementation.Dispose();
        }

        public void UpdateSecret(string newSecret, string reason)
        {
            _rabbitMqClientImplementation.UpdateSecret(newSecret, reason);
        }

        public void Abort()
        {
            _rabbitMqClientImplementation.Abort();
        }

        public void Abort(ushort reasonCode, string reasonText)
        {
            _rabbitMqClientImplementation.Abort(reasonCode, reasonText);
        }

        public void Abort(TimeSpan timeout)
        {
            _rabbitMqClientImplementation.Abort(timeout);
        }

        public void Abort(ushort reasonCode, string reasonText, TimeSpan timeout)
        {
            _rabbitMqClientImplementation.Abort(reasonCode, reasonText, timeout);
        }

        public void Close()
        {
            _rabbitMqClientImplementation.Close();
        }

        public void Close(ushort reasonCode, string reasonText)
        {
            _rabbitMqClientImplementation.Close(reasonCode, reasonText);
        }

        public void Close(TimeSpan timeout)
        {
            _rabbitMqClientImplementation.Close(timeout);
        }

        public void Close(ushort reasonCode, string reasonText, TimeSpan timeout)
        {
            _rabbitMqClientImplementation.Close(reasonCode, reasonText, timeout);
        }

        public IModel CreateModel()
        {
            return _rabbitMqClientImplementation.CreateModel();
        }

        public void HandleConnectionBlocked(string reason)
        {
            _rabbitMqClientImplementation.HandleConnectionBlocked(reason);
        }

        public void HandleConnectionUnblocked()
        {
            _rabbitMqClientImplementation.HandleConnectionUnblocked();
        }

        public ushort ChannelMax => _rabbitMqClientImplementation.ChannelMax;

        public IDictionary<string, object> ClientProperties => _rabbitMqClientImplementation.ClientProperties;

        public ShutdownEventArgs CloseReason => _rabbitMqClientImplementation.CloseReason;

        public AmqpTcpEndpoint Endpoint => _rabbitMqClientImplementation.Endpoint;

        public uint FrameMax => _rabbitMqClientImplementation.FrameMax;

        public TimeSpan Heartbeat => _rabbitMqClientImplementation.Heartbeat;

        public bool IsOpen => _rabbitMqClientImplementation.IsOpen;

        public AmqpTcpEndpoint[] KnownHosts => _rabbitMqClientImplementation.KnownHosts;

        public IProtocol Protocol => _rabbitMqClientImplementation.Protocol;

        public IDictionary<string, object> ServerProperties => _rabbitMqClientImplementation.ServerProperties;

        public IList<ShutdownReportEntry> ShutdownReport => _rabbitMqClientImplementation.ShutdownReport;

        public string ClientProvidedName => _rabbitMqClientImplementation.ClientProvidedName;

        public event EventHandler<CallbackExceptionEventArgs> CallbackException
        {
            add => _rabbitMqClientImplementation.CallbackException += value;
            remove => _rabbitMqClientImplementation.CallbackException -= value;
        }

        public event EventHandler<ConnectionBlockedEventArgs> ConnectionBlocked
        {
            add => _rabbitMqClientImplementation.ConnectionBlocked += value;
            remove => _rabbitMqClientImplementation.ConnectionBlocked -= value;
        }

        public event EventHandler<ShutdownEventArgs> ConnectionShutdown
        {
            add => _rabbitMqClientImplementation.ConnectionShutdown += value;
            remove => _rabbitMqClientImplementation.ConnectionShutdown -= value;
        }

        public event EventHandler<EventArgs> ConnectionUnblocked
        {
            add => _rabbitMqClientImplementation.ConnectionUnblocked += value;
            remove => _rabbitMqClientImplementation.ConnectionUnblocked -= value;
        }
    }
}