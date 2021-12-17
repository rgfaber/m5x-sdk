using System;
using System.Net;

namespace M5x.Networking;

/// <summary>
///     AsyncTcpClientEventArgs.
/// </summary>
public class AsyncTcpClientEventArgs : EventArgs
{
    #region Members

    #region Privates

    #endregion


    #region Publics

    // Publics members goes here.

    #endregion

    #endregion

    #region Properties

    /// <summary>
    ///     Remote Endpoint.
    /// </summary>
    public IPEndPoint RemoteEndPoint { get; }

    /// <summary>
    ///     Connection status.
    /// </summary>
    public SocketState Status { get; }

    /// <summary>
    ///     Exception when Status == Error.
    /// </summary>
    public Exception Exception { get; internal set; }

    /// <summary>
    ///     Data received.
    /// </summary>
    public byte[] Data { get; internal set; }

    /// <summary>
    ///     Size of data received.
    /// </summary>
    public int Length { get; internal set; }

    #endregion

    #region Events

    // Events and delegates goes here.

    #endregion

    #region Constructor(s) & Destructor

    /// <summary>
    ///     Create a new AsyncTcpClientEventArgs (used in Connected event).
    /// </summary>
    /// <param name="remoteEndPoint">Remote host</param>
    /// <param name="status">Status</param>
    public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, SocketState status)
    {
        RemoteEndPoint = remoteEndPoint;
        Status = status;
    }

    /// <summary>
    ///     Create a new AsyncTcpClientEventArgs (used when data are received).
    /// </summary>
    /// <param name="remoteEndPoint">Remote host</param>
    /// <param name="length">Size of received data</param>
    /// <param name="data">Received data</param>
    public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, int length, byte[] data)
    {
        RemoteEndPoint = remoteEndPoint;
        Status = SocketState.Listening;
        Length = length;
        Data = data;
    }

    /// <summary>
    ///     Create a new AsyncTcpClientEventArgs (used when exception occurred).
    /// </summary>
    /// <param name="remoteEndPoint">Remote host</param>
    /// <param name="ex">Exception</param>
    public AsyncTcpClientEventArgs(IPEndPoint remoteEndPoint, Exception ex)
    {
        RemoteEndPoint = remoteEndPoint;
        Status = SocketState.Error;
        Exception = ex;
    }

    #endregion

    #region Methods

    #region Statics

    // Statics methods goes here.

    #endregion

    #region Publics

    // Publics methods goes here.

    #endregion

    #region Privates

    // Privates methods goes here.

    #endregion

    #endregion
}