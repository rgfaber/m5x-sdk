using System;
using System.Net.Sockets;
using System.Threading;

namespace M5x.Networking;

/// <summary>
///     Handle a single connection (TcpClient and associated Thread).
/// </summary>
public class TcpConnectionHandle
{
    private Thread _connectionThread; // Thread that running the TcpClient.

    /// <summary>
    ///     Create an empty TcpConnectionHandle.
    /// </summary>
    public TcpConnectionHandle()
    {
        Uid = Guid.NewGuid();
    }

    /// <summary>
    ///     Create a new TcpConnectionHandle.
    /// </summary>
    /// <param name="pTh">Thread in which ConnectionClient is running</param>
    public TcpConnectionHandle(Thread pTh)
    {
        _connectionThread = pTh;
        Id = _connectionThread.ManagedThreadId;
        Uid = Guid.NewGuid();
    }

    /// <summary>
    ///     Create a new TcpConnectionHandle.
    /// </summary>
    /// <param name="pTh">Thread in which ConnectionClient is running</param>
    /// <param name="pTc">TcpClient used to handle communication between client end server</param>
    public TcpConnectionHandle(Thread pTh, TcpClient pTc)
    {
        _connectionThread = pTh;
        ConnectionClient = pTc;
        Id = _connectionThread.ManagedThreadId;
        Uid = Guid.NewGuid();
    }

    /// <summary>
    ///     Create a new TcpConnectionHandle.
    /// </summary>
    /// <param name="pTh">Thread in which ConnectionClient is running</param>
    /// <param name="pTc">TcpClient used to handle communication between client end server</param>
    /// <param name="pId">Ident of a session</param>
    public TcpConnectionHandle(Thread pTh, TcpClient pTc, int pId)
    {
        _connectionThread = pTh;
        ConnectionClient = pTc;
        Id = pId;
        Uid = Guid.NewGuid();
    }

    /// <summary>
    ///     Gets or sets the session ident (default is the ManagedThreadId when ConnectionThread is set).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets the uid. Same goal of <see cref="Id" /> but using a Guid type.
    /// </summary>
    /// <value>The uid.</value>
    public Guid Uid { get; }

    /// <summary>
    ///     Thread in which ConnectioClient is running.
    /// </summary>
    public Thread ConnectionThread
    {
        get => _connectionThread;
        set
        {
            _connectionThread = value;
            Id = _connectionThread.ManagedThreadId;
        }
    }

    /// <summary>
    ///     TcpClient used to handle communication with remote client.
    /// </summary>
    public TcpClient ConnectionClient { get; set; }
} // End TcpConnectionHandle