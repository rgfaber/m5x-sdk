// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-17-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-20-2014
// ***********************************************************************
// <copyright file="StreamHelper.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.IO;

namespace M5x.Streams;

/// <summary>
///     Class StreamHelper.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     Reads to end.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>System.Byte[][].</returns>
    public static byte[] ToBytes(this Stream stream)
    {
        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            var readBuffer = new byte[4096];

            var totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead != readBuffer.Length) continue;
                var nextByte = stream.ReadByte();
                if (nextByte == -1) continue;
                var temp = new byte[readBuffer.Length * 2];
                Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                readBuffer = temp;
                totalBytesRead++;
            }

            var buffer = readBuffer;
            if (readBuffer.Length == totalBytesRead) return buffer;
            buffer = new byte[totalBytesRead];
            Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            return buffer;
        }
        finally
        {
            if (stream.CanSeek) stream.Position = originalPosition;
        }
    }


    /// <summary>
    ///     Ases the string.
    /// </summary>
    /// <param name="sIn">The s in.</param>
    /// <returns>System.String.</returns>
    public static string AsString(this Stream sIn)
    {
        if (sIn.CanSeek)
            sIn.Position = 0;
        var sr = new StreamReader(sIn);
        var s = sr.ReadToEnd();
        return s;
    }

    /// <summary>
    ///     Ases the base64 string.
    /// </summary>
    /// <param name="sIn">The s in.</param>
    /// <returns>System.String.</returns>
    public static string AsBase64String(this Stream sIn)
    {
        return Convert.ToBase64String(sIn.ToByteArray());
    }


    /// <summary>
    ///     Copies the stream.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="output">The output.</param>
    /// <remarks></remarks>
    public static void CopyStream(Stream input, Stream output)
    {
        var b = new byte[32768];
        int r;
        while ((r = input.Read(b, 0, b.Length)) > 0)
            output.Write(b, 0, r);
    }


    /// <summary>
    ///     Toes the byte array.
    /// </summary>
    /// <param name="sIn">The s in.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static byte[] ToByteArray(this Stream sIn)
    {
        if (sIn == null) return null;
        sIn.Seek(0, 0);
        var mem = new MemoryStream();
        CopyStream(sIn, mem);
        // getting the internal buffer (no additional copying)
        var buffer = mem.GetBuffer();
        var length = mem.Length; // the actual length of the data 
        // (the array may be longer)

        // if you need the array to be exactly as long as the data
        return mem.ToArray(); // makes another copy
    }
}