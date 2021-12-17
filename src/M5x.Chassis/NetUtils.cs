using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace M5x.Chassis;

/// <summary>
///     Network utilities.
/// </summary>
public static class NetUtils
{
    #region Static Methods

    /// <summary>
    ///     Return IP Adresses used by a host.
    /// </summary>
    /// <remarks>
    ///     On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
    /// </remarks>
    /// <param name="hostName">host</param>
    /// <returns>IP addresses</returns>
    public static IPAddress[] GetIp(string hostName)
    {
        var ipEntry = Dns.GetHostEntry(hostName);
        var addr = ipEntry.AddressList;

        return addr;
    }

    /// <summary>
    ///     Return IP Adresses used by a localhost.
    /// </summary>
    /// <remarks>
    ///     On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
    /// </remarks>
    /// <returns>IP addresses</returns>
    public static IPAddress[] GetIp()
    {
        var hostName = Dns.GetHostName();

        return GetIp(hostName);
    }

    //protected static void DNSLookup(string hostNameOrAddress)
    //{
    //    Console.WriteLine("Lookup: {0}\n", hostNameOrAddress);

    //    IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
    //    Console.WriteLine("  Host Name: {0}", hostEntry.HostName);

    //    IPAddress[] ips = hostEntry.AddressList;
    //    foreach (IPAddress ip in ips)
    //    {
    //        Console.WriteLine("  Address: {0}", ip);
    //    }

    //    Console.WriteLine();
    //} 

    /// <summary>
    ///     Return V4 IP Adresses used by a localhost.
    /// </summary>
    /// <remarks>
    ///     On system which support IPv6, only IPv4 adresses are returned in the same array for all interfaces.
    /// </remarks>
    /// <returns>IP addresses</returns>
    public static IPAddress[] GetIPv4AssociatedWithLocalHost()
    {
        var hostName = Dns.GetHostName();

        const bool pv4Only = true;
        return GetIPs(hostName, pv4Only);
    }


    /// <summary>
    ///     Gets the first ip4.
    /// </summary>
    /// <param name="hostName">Name of the host.</param>
    /// <returns></returns>
    public static string GetFirstIp4(string hostName)
    {
        const bool pv4Only = true;
        var ips = GetIPs(hostName, pv4Only);
        if (ips.Length > 0) return ips[0].ToString();
        return string.Empty;
    }

    /// <summary>
    ///     Return IP Adresses used by a host.
    /// </summary>
    /// <param name="hostName">host</param>
    /// <param name="IPv4Only">if set to <c>true</c> [I PV4 only].</param>
    /// <returns>IP addresses</returns>
    /// <remarks>
    ///     On system which support IPv6, IPv4 and IPv6 are returned in the same array for all interfaces.
    /// </remarks>
    public static IPAddress[] GetIPs(string hostName, bool pv4Only)
    {
        var hostEntry = Dns.GetHostEntry(hostName);
        var addressList = hostEntry.AddressList;

        if (pv4Only)
        {
            var ipList = new List<IPAddress>();
            foreach (var iPAddress in addressList)
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                    ipList.Add(iPAddress);
            return ipList.ToArray();
        }

        return addressList;
    }

    /// <summary>
    ///     Return the hostname (Dns.GetHostName()).
    /// </summary>
    /// <returns>Hostname</returns>
    public static string GetHostName()
    {
        return Dns.GetHostName();
    }

    /// <summary>
    ///     Return local IPv4 Addresses declared by all existing network interfaces.
    /// </summary>
    /// <returns>Declared unicast IPv4 Addresses</returns>
    public static IPAddress[] GetIPv4()
    {
        List<IPAddress> addr; // Hold existing Ip v4 addresses.
        NetworkInterface[] interfaces; // Hold network interfaces.

        // Get list of all interfaces.
        interfaces = NetworkInterface.GetAllNetworkInterfaces();
        addr = new List<IPAddress>();

        // Loop through interfaces.
        foreach (var iface in interfaces)
        {
            // Create collection to hold IP information for interfaces.
            UnicastIPAddressInformationCollection ips;

            // Get list of all unicast IPs from current interface.
            ips = iface.GetIPProperties().UnicastAddresses;

            // Loop through IP address collection.
            foreach (var ip in ips)
                // Check IP address for IPv4.
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    addr.Add(ip.Address);
        }

        return addr.ToArray();
    }


    ///// <summary>
    ///// Declare an URL to be used with HttpApi.<br/>
    ///// Windows Xp SP2 and Windows 2003 introduce an http layer named HttpApi which limit the use of Http only for
    ///// Administrators and for registered URL.
    ///// </summary>
    ///// <param name="networkURL">URL to reserve with HttpApi</param>
    ///// <param name="securityDescriptor">DACL string (see Psbe.Common.Shared.Utils.CreateSddl)</param>
    //public static void ReserveURL(string networkURL, string securityDescriptor)
    //{
    //    uint retVal = (uint)HttpApi.ErrorCodes.NO_ERROR; // NO_ERROR = 0
    //    HttpApi.HTTPAPI_VERSION httpApiVersion = new HttpApi.HTTPAPI_VERSION(1, 0);

    //    retVal = HttpApi.HttpInitialize(httpApiVersion, HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

    //    if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
    //    {
    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY keyDesc = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY(networkURL);
    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM paramDesc = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor);

    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET inputConfigInfoSet = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET();
    //        inputConfigInfoSet.KeyDesc = keyDesc;
    //        inputConfigInfoSet.ParamDesc = paramDesc;

    //        IntPtr pInputConfigInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET)));
    //        Marshal.StructureToPtr(inputConfigInfoSet, pInputConfigInfo, false);

    //        retVal = HttpApi.HttpSetServiceConfiguration(IntPtr.Zero,
    //                                                     HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
    //                                                     pInputConfigInfo,
    //                                                     Marshal.SizeOf(inputConfigInfoSet),
    //                                                     IntPtr.Zero);

    //        if ((uint)HttpApi.ErrorCodes.ERROR_ALREADY_EXISTS == retVal)
    //        {  // ERROR_ALREADY_EXISTS = 183
    //            retVal = HttpApi.HttpDeleteServiceConfiguration(IntPtr.Zero,
    //                                                            HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
    //                                                            pInputConfigInfo,
    //                                                            Marshal.SizeOf(inputConfigInfoSet),
    //                                                            IntPtr.Zero);

    //            if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
    //            {
    //                retVal = HttpApi.HttpSetServiceConfiguration(IntPtr.Zero,
    //                                                             HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
    //                                                             pInputConfigInfo,
    //                                                             Marshal.SizeOf(inputConfigInfoSet),
    //                                                             IntPtr.Zero);
    //            }
    //        }

    //        Marshal.FreeCoTaskMem(pInputConfigInfo);
    //        HttpApi.HttpTerminate(HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
    //    }

    //    if ((uint)HttpApi.ErrorCodes.NO_ERROR != retVal)
    //    {
    //        throw new Win32Exception(Convert.ToInt32(retVal));
    //    }
    //}


    ///// <summary>
    ///// Release an URL used with HttpApi.<br />
    ///// Windows Xp SP2 and Windows 2003 introduce an http layer named HttpApi which limit the use of Http only for
    ///// Administrators and for registered URL.
    ///// </summary>
    ///// <param name="networkURL">URL reserved with HttpApi to release</param>
    ///// <param name="securityDescriptor">DACL string (see Psbe.Common.Shared.Utils.CreateSddl)</param>
    //public static void FreeURL(string networkURL, string securityDescriptor)
    //{
    //    uint retVal = (uint)HttpApi.ErrorCodes.NO_ERROR;
    //    HttpApi.HTTPAPI_VERSION httpApiVersion = new HttpApi.HTTPAPI_VERSION(1, 0);

    //    retVal = HttpApi.HttpInitialize(httpApiVersion, HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

    //    if ((uint)HttpApi.ErrorCodes.NO_ERROR == retVal)
    //    {
    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY urlAclKey = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_KEY(networkURL);
    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM urlAclParam = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_PARAM(securityDescriptor);

    //        HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET urlAclSet = new HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET();
    //        urlAclSet.KeyDesc = urlAclKey;
    //        urlAclSet.ParamDesc = urlAclParam;

    //        IntPtr configInformation = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(HttpApi.HTTP_SERVICE_CONFIG_URLACL_SET)));
    //        Marshal.StructureToPtr(urlAclSet, configInformation, false);
    //        int configInformationSize = Marshal.SizeOf(urlAclSet);

    //        retVal = HttpApi.HttpDeleteServiceConfiguration(IntPtr.Zero,
    //                                                        HttpApi.HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo,
    //                                                        configInformation,
    //                                                        configInformationSize,
    //                                                        IntPtr.Zero);

    //        Marshal.FreeCoTaskMem(configInformation);

    //        HttpApi.HttpTerminate(HttpApi.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
    //    }

    //    if ((uint)HttpApi.ErrorCodes.NO_ERROR != retVal)
    //    {
    //        throw new Win32Exception(Convert.ToInt32(retVal));
    //    }
    //}


    /// <summary>
    ///     Get the first unused port above 1024.
    /// </summary>
    /// <param name="localAddr">Ip to bind to</param>
    /// <returns>Port number, 0 in case of error</returns>
    public static int GetFirstUnusedPort(IPAddress localAddr)
    {
        for (var p = 1024; p <= IPEndPoint.MaxPort; p++)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // If the port is used, an exception 10048 will be raised when we try to bind to...
                s.Bind(new IPEndPoint(localAddr, p));
                s.Close();
                return p;
            }
            catch (SocketException ex)
            {
                // Address in use ?
                if (ex.ErrorCode == 10048)
                    continue;
                return 0;
            }
        }

        return 0;
    }

    /// <summary>
    ///     Get the first unused port above startPort.
    /// </summary>
    /// <param name="localAddr">Ip to bind to</param>
    /// <param name="startPort">Starting port</param>
    /// <returns>Port number, 0 in case of error</returns>
    public static int GetFirstUnusedPort(IPAddress localAddr, int startPort)
    {
        for (var p = startPort; p <= IPEndPoint.MaxPort; p++)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // If the port is used, an exception 10048 will be raised when we try to bind to...
                s.Bind(new IPEndPoint(localAddr, p));
                s.Close();
                return p;
            }
            catch (SocketException ex)
            {
                // Address in use ?
                if (ex.ErrorCode == 10048)
                    continue;
                return 0;
            }
        }

        return 0;
    }


    /// <summary>
    ///     Get the first unused port between startPort and endPort.
    /// </summary>
    /// <param name="localAddr">Ip to bind to</param>
    /// <param name="startPort">Starting port</param>
    /// <param name="endPort">Ending port</param>
    /// <returns>Port number, 0 in case of error</returns>
    public static int GetFirstUnusedPort(IPAddress localAddr, int startPort, int endPort)
    {
        for (var p = startPort; p <= endPort; p++)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // If the port is used, an exception 10048 will be raised when we try to bind to...
                s.Bind(new IPEndPoint(localAddr, p));
                s.Close();
                return p;
            }
            catch (SocketException ex)
            {
                // Address in use ?
                if (ex.ErrorCode == 10048)
                    continue;
                return 0;
            }
        }

        return 0;
    }

    /// <summary>
    ///     Verify is a computer + port is connectable on the network.
    /// </summary>
    /// <param name="addr">Ip to bind to</param>
    /// <param name="port">Starting port</param>
    /// <returns>Port number, 0 in case of error</returns>
    public static bool IsConnectable(string addr, int port)
    {
        var connector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //      IPEndPoint host = new IPEndPoint(IPAddress.Parse(addr), (int)port);

        try
        {
            connector.Blocking = false;
            connector.Connect(addr, port);

            if (connector.Poll(1000, SelectMode.SelectRead)) return true;

            return false;
            throw new Exception("Timeout detected");
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     Compresses the given data using GZip algorithm.
    /// </summary>
    /// <param name="data">The data to be compressed.</param>
    /// <returns>The compressed data</returns>
    public static byte[] CompressGZip(byte[] data)
    {
        Stream memoryStream = new MemoryStream();
        var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
        //Stream gZipStream = new GZipOutputStream(memoryStream);  // Does not work, only returns a header of 10 bytes
        gZipStream.Write(data, 0, data.Length);
        gZipStream.Close();

        // Reposition memoryStream to the beginning
        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, (int)memoryStream.Length);

        memoryStream.Close();

        return compressedData;
    }

    /// <summary>
    ///     Decompresses the given data using GZip algorithm.
    /// </summary>
    /// <param name="data">The data to be decompressed.</param>
    /// <returns>The decompressed data</returns>
    public static byte[] DecompressGZip(byte[] data)
    {
        Stream compressedMemoryStream = new MemoryStream(data);
        var gZipStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress, true);
        Stream decompressedMemoryStream = new MemoryStream(data.Length);
        int byteRead;
        //while ((byteRead = compressedMemoryStream.ReadByte()) != -1)
        while ((byteRead = gZipStream.ReadByte()) != -1) decompressedMemoryStream.WriteByte((byte)byteRead);
        gZipStream.Close();
        compressedMemoryStream.Close();

        // Reposition memoryStream to the beginning
        decompressedMemoryStream.Position = 0;

        var decompressedData = new byte[decompressedMemoryStream.Length];
        decompressedMemoryStream.Read(decompressedData, 0, (int)decompressedMemoryStream.Length);
        decompressedMemoryStream.Close();

        return decompressedData;
    }


    /// <summary>
    ///     Gets the local ip address.
    /// </summary>
    /// <returns>System.String.</returns>
    /// <exception cref="System.Exception">No network adapters with an IPv4 address in the system!</exception>
    //public static string GetLocalIpAddress()
    //{
    //    var host = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (var ip in host.AddressList)
    //    {
    //        if (ip.AddressFamily == AddressFamily.InterNetwork)
    //        {
    //            return ip.ToString();
    //        }
    //    }
    //    throw new Exception("No network adapters with an IPv4 address in the system!");
    //}
    public static IPAddress GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip;
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    #endregion
} // End NetUtils