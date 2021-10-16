using System.Net;

namespace M5x.Networking
{
    /// <summary>
    ///     Class for exchanging data from the server (TCP or UDP) and the client
    ///     application via event.
    /// </summary>
    public class MessageData
    {
        /// <summary>
        ///     Message data.
        /// </summary>
        public byte[] Message;

        /// <summary>
        ///     Length of received data.
        /// </summary>
        public int MessageLength;

        /// <summary>
        ///     Remote IP Address.
        /// </summary>
        public EndPoint RemoteEndPoint;

        /// <summary>
        ///     Create a new MessageData object.
        /// </summary>
        /// <param name="pMsg">Message content</param>
        /// <param name="pMsgLength">Message length</param>
        /// <param name="pRemoteEndPoint">Remote endpoint</param>
        /// <param name="pCnxHandle">Handle of the TCP connection</param>
        public MessageData(byte[] pMsg, int pMsgLength, EndPoint pRemoteEndPoint, TcpConnectionHandle pCnxHandle)
        {
            Message = pMsg;
            MessageLength = pMsgLength;
            RemoteEndPoint = pRemoteEndPoint;
            ConnectionHandle = pCnxHandle;
        }

        /// <summary>
        ///     Create a new MessageData object.
        /// </summary>
        public MessageData()
        {
        }

        /// <summary>
        ///     TCP Connection handle.
        /// </summary>
        public TcpConnectionHandle ConnectionHandle { get; }
    } // end MessageData
}