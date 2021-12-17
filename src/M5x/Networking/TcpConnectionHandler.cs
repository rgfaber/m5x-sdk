using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace M5x.Networking;

/// <summary>
///     List of TcpConnectionHandle.
/// </summary>
public class TcpConnectionHandler : List<TcpConnectionHandle>
{
    /// <summary>
    ///     Retrieve a TcpConnectionHandler identified by pId
    /// </summary>
    /// <param name="pId">Ident of the TcpConnectionHandle</param>
    /// <returns>Null if not found</returns>
    public TcpConnectionHandle GetConnectionHandle(int pId)
    {
        for (var i = 0; i < Count; i++)
            if (this[i].Id == pId)
                return this[i];
        return null;
    }

    /// <summary>
    ///     Retrieve a TcpConnectionHandle for thread pTh.
    ///     Same as GetConnectionHandle(myThread.ManagedThreadId);
    /// </summary>
    /// <param name="pTh">Thread</param>
    /// <returns>Null if not found</returns>
    public TcpConnectionHandle GetConnectionHandle(Thread pTh)
    {
        return GetConnectionHandle(pTh.ManagedThreadId);
    }

    /// <summary>
    ///     Retrieve a TcpConnectionHandle for TcpClient pTc.
    /// </summary>
    /// <param name="pTc">TcpClient</param>
    /// <returns>Null if not found</returns>
    public TcpConnectionHandle GetConnectionHandle(TcpClient pTc)
    {
        for (var i = 0; i < Count; i++)
            if (this[i].ConnectionClient == pTc)
                return this[i];
        return null;
    }

    /// <summary>
    ///     Return the index in the collection of TcpConnectionHandle identified by pId.
    /// </summary>
    /// <param name="pId">Connection Ident</param>
    /// <returns>-1 if not found</returns>
    public int IndexOf(int pId)
    {
        for (var i = 0; i < Count; i++)
            if (this[i].Id == pId)
                return i;
        return -1;
    }

    /// <summary>
    ///     Return the index in the collection of TcpConnectionHandle identified by pTh.
    /// </summary>
    /// <param name="pTh">Connection Thread</param>
    /// <returns>-1 if not found</returns>
    public int IndexOf(Thread pTh)
    {
        return IndexOf(pTh.ManagedThreadId);
    }

    /// <summary>
    ///     Return the index in the collection of TcpConnectionHandle identified by pTc.
    /// </summary>
    /// <param name="pTc">Connection TcpClient</param>
    /// <returns>-1 if not found</returns>
    public int IndexOf(TcpClient pTc)
    {
        for (var i = 0; i < Count; i++)
            if (this[i].ConnectionClient == pTc)
                return i;
        return -1;
    }

    /// <summary>
    ///     Remove from the collection the TcpConnectionHandle identified by pId.
    /// </summary>
    /// <param name="pId">Connection Ident</param>
    public void Remove(int pId)
    {
        var idx = IndexOf(pId);
        if (idx != -1) Remove(this[idx]);
    }

    /// <summary>
    ///     Remove from the collection the TcpConnectionHandle identified by pTh.
    /// </summary>
    /// <param name="pTh">Connection Thread</param>
    public void Remove(Thread pTh)
    {
        var idx = IndexOf(pTh);
        if (idx != -1) Remove(this[idx]);
    }

    /// <summary>
    ///     Remove from the collection the TcpConnectionHandle identified by pTc.
    /// </summary>
    /// <param name="pTc">Connection TcpClient</param>
    public void Remove(TcpClient pTc)
    {
        var idx = IndexOf(pTc);
        if (idx != -1) Remove(this[idx]);
    }
} // End TcpConnectionHandler