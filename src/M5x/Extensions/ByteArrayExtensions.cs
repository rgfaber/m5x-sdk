using System;
using System.IO;

namespace M5x.Extensions;

public static class ByteArrayExtensions
{
    /// <summary>
    ///     Encodes the binary data to base64 (UTF8).
    /// </summary>
    /// <param name="data">The data to be encoded.</param>
    /// <returns>The data encoded in base64 (UTF8)</returns>
    public static byte[] EncodeBase64(this byte[] data)
    {
        // Convert to base64 (string)
        var base64String = Convert.ToBase64String(data);

        // Convert string to byte array (UTF8)
        Stream memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        streamWriter.Write(base64String);
        streamWriter.Flush();

        var encodedData = new byte[memoryStream.Length];
        // Reposition memory stream
        memoryStream.Position = 0;
        memoryStream.Read(encodedData, 0, (int)memoryStream.Length);

        // Close streams
        streamWriter.Close();
        memoryStream.Close();

        // Return the byte array
        return encodedData;
    }

    /// <summary>
    ///     Decodes the binary data from base64 (UTF8).
    /// </summary>
    /// <param name="data">The data to be decoded and expected to be encoded in base64 (UTF8).</param>
    /// <returns>The decoded binary data</returns>
    public static byte[] DecodeBase64(this byte[] data)
    {
        // Reconstruct a string from input data
        Stream memoryStream = new MemoryStream(data);
        var streamReader = new StreamReader(memoryStream);
        var base64String = streamReader.ReadToEnd();
        // Close streams
        streamReader.Close();
        memoryStream.Close();

        // Convert from base64
        var decodedData = Convert.FromBase64String(base64String);

        return decodedData;
    }


    /// <summary>
    ///     Converts the byte array to stream.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static Stream ToStream(this byte[] input)
    {
        var sOut = new MemoryStream();
        sOut.Write(input, 0, input.Length);
        sOut.Seek(0, 0);
        return sOut;
    }
}