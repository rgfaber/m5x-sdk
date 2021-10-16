using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Serilog;

namespace M5x.CEQS.Schema.Streams
{
    /// <summary>
    /// </summary>
    /// <remarks></remarks>
    public static class StreamUtils
    {
        /// <summary>
        ///     Gets the embedded file.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static void SerializeTo<T>(this T @object, Stream stream)
        {
            new BinaryFormatter().Serialize(stream, @object); // serialize o not typeof(T)
        }

        public static T Deserialize<T>(this Stream stream)
        {
            return (T) new BinaryFormatter().Deserialize(stream);
        }


        public static Stream ToStream(this string s)
        {
            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Seek(0, 0);
                return stream;
            }
            catch (Exception e)
            {
                Log.Fatal(e, e.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}