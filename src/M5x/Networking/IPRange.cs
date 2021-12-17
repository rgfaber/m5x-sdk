using System.Net;

namespace M5x.Networking;

/// <summary>
///     Declare a range of IP Addresses.
/// </summary>
public class IpRange
{
    private uint _ipEnd; // Last IP address of the range.
    private uint _ipStart; // First IP address of the range.

    /// <summary>
    ///     Declare a range of IP Addresses.
    /// </summary>
    /// <param name="pIpStart">Start address (numerical form)</param>
    /// <param name="pIpEnd">End address (numerical form)</param>
    public IpRange(uint pIpStart, uint pIpEnd)
    {
        _ipStart = pIpStart;
        _ipEnd = pIpEnd;
    }

    /// <summary>
    ///     Declare a range of IP Addresses.
    /// </summary>
    /// <param name="pIpStart">Start address (dotted form)</param>
    /// <param name="pIpEnd">End address (dotted form)</param>
    public IpRange(string pIpStart, string pIpEnd)
    {
        _ipStart = IpAddressToUInt(pIpStart);
        _ipEnd = IpAddressToUInt(pIpEnd);
    }

    /// <summary>
    ///     Declare a range of IP Addresses.
    /// </summary>
    /// <param name="pIpStart">Start address</param>
    /// <param name="pIpEnd">End address</param>
    public IpRange(IPAddress pIpStart, IPAddress pIpEnd)
    {
        _ipStart = IpAddressToUInt(pIpStart);
        _ipEnd = IpAddressToUInt(pIpEnd);
    }

    /// <summary>
    ///     Start IP address of the range.
    /// </summary>
    public IPAddress IpStart
    {
        get => UIntToIpAddress(_ipStart);
        set => _ipStart = IpAddressToUInt(value);
    }

    /// <summary>
    ///     End IP address of the range.
    /// </summary>
    public IPAddress IpEnd
    {
        get => UIntToIpAddress(_ipEnd);
        set => _ipEnd = IpAddressToUInt(value);
    }

    /// <summary>
    ///     Return if an IP address is betwwen the IP range.
    ///     -1 = lower than the range.
    ///     0 = in the range.
    ///     1 = upper than the range.
    /// </summary>
    /// <param name="pIp">Address to check</param>
    /// <returns>position of the address compare with the range</returns>
    public int Contains(IPAddress pIp)
    {
        var ip = IpAddressToUInt(pIp);

        return Contains(ip);
    }

    /// <summary>
    ///     Return if an IP address is betwwen the IP range.
    ///     -1 = lower than the range.
    ///     0 = in the range.
    ///     1 = upper than the range.
    /// </summary>
    /// <param name="pIp">Address to check</param>
    /// <returns>position of the address compare with the range</returns>
    public int Contains(string pIp)
    {
        var ip = IpAddressToUInt(pIp);

        return Contains(ip);
    }

    /// <summary>
    ///     Return if an IP address is betwwen the IP range.
    ///     -1 = lower than the range.
    ///     0 = in the range.
    ///     1 = upper than the range.
    /// </summary>
    /// <param name="pIp">Address to check</param>
    /// <returns>position of the address compare with the range</returns>
    public int Contains(uint pIp)
    {
        if (_ipStart <= pIp && _ipEnd >= pIp) return 0;

        if (_ipStart > pIp) return -1;

        if (_ipEnd < pIp) return 1;

        return -1;
    }

    /// <summary>
    ///     Converts a an IP address to a uint.
    ///     This encoding is proper and can be used with other networking functions such
    ///     as the System.Net.IPAddress class.
    /// </summary>
    /// <param name="pIp">The Ip address to convert.</param>
    /// <returns>Returns a uint representation of the IP address.</returns>
    public static uint IpAddressToUInt(IPAddress pIp)
    {
        // Split ip address in 4 element array consisting of the octets in the IP string.
        var byteIp = pIp.GetAddressBytes();

        // Shift the first octet over 24 bits so that it's in the highest position.
        // Thus if addressOctets[0] is 0x8D, addressValue will be 0x8D000000.
        var ip = (uint)byteIp[3] << 24;
        ip += (uint)byteIp[2] << 16;
        ip += (uint)byteIp[1] << 8;
        ip += byteIp[0];

        return ip;
    }


    /// <summary>
    ///     Converts a string representation of an IP address to a uint.
    ///     This encoding is proper and can be used with other networking functions such
    ///     as the System.Net.IPAddress class.
    /// </summary>
    /// <param name="pIp">The Ip address to convert.</param>
    /// <returns>Returns a uint representation of the IP address.</returns>
    public static uint IpAddressToUInt(string pIp)
    {
        var oIp = IPAddress.Parse(pIp);

        return IpAddressToUInt(oIp);
    }


    /// <summary>
    ///     Convert a uint IP address to regular dotted representation.
    /// </summary>
    /// <param name="pIp">A uint representation of the IP address to convert</param>
    /// <returns>Returns a string dotted representation of the IP address</returns>
    public static string UIntToCanonicalIpAddress(uint pIp)
    {
        return new IPAddress(pIp).ToString();
    }

    /// <summary>
    ///     Convert a uint IP address to regular dotted representation.
    /// </summary>
    /// <param name="pIp">A uint representation of the IP address to convert</param>
    /// <returns>Returns a string dotted representation of the IP address</returns>
    public static IPAddress UIntToIpAddress(uint pIp)
    {
        return new IPAddress(pIp);
    }

    /// <summary>
    ///     This encodes the string representation of an IP address to a uint,
    ///     but backwards so that it can be used to compare addresses.
    ///     This function is used internally for comparison and is not valid
    ///     for valid encoding of IP address information.
    /// </summary>
    /// <param name="pIp">A string representation of the IP address to convert</param>
    /// <returns>Returns a backwards uint representation of the string.</returns>
    private static uint IpAddressToUIntBackwards(string pIp)
    {
        var oIp = IPAddress.Parse(pIp);
        var byteIp = oIp.GetAddressBytes();


        var ip = (uint)byteIp[0] << 24;
        ip += (uint)byteIp[1] << 16;
        ip += (uint)byteIp[2] << 8;
        ip += byteIp[3];

        return ip;
    }
} // End IPRange