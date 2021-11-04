using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace M5x.Extensions
{
    /// <summary>
    ///     Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNumeric(this string strTextEntry)
        {
            var objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strTextEntry);
        }


        /// <summary>
        ///     Concatenates the specified string array into one string
        ///     containing the comma separated list of the string values.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>System.String.</returns>
        public static string Concatenate(this string[] array)
        {
            var result = string.Empty;
            foreach (var element in array)
            {
                if (string.IsNullOrEmpty(result) == false) result += ", ";
                result += element;
            }

            return result;
        }

        /// <summary>
        ///     Base64s the decode.
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data.</param>
        /// <returns>System.String.</returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        ///     Base64s the encode.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>System.String.</returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        ///     Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Compress(this string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        ///     Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string Decompress(this string compressedText)
        {
            var gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }


        // Convert the string to Pascal case.
        public static string ToPascalCase(this string theString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (theString.Length < 2) return theString.ToUpper();

            // Split the string into words.
            var words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.

            return words.Aggregate("",
                (current, word) => current + word.Substring(0, 1).ToUpper() + word.Substring(1));
        }


        // Convert the string to camel case.
        public static string ToCamelCase(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return theString;
            // If there are 0 or 1 characters, just return the string.
            if (theString.Length < 2) return theString;
            // Split the string into words.
            var words = theString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            var result = words[0].ToLower();
            for (var i = 1; i < words.Length; i++)
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);

            return result;
        }


        // Capitalize the first character and add a space before
        // each capitalized letter (except the first character).
        public static string ToProperCase(this string theString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (string.IsNullOrWhiteSpace(theString)) return theString;
            if (theString.Length < 2) return theString.ToUpper();

            // Start with the first character.
            var result = theString.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (var i = 1; i < theString.Length; i++)
            {
                if (char.IsUpper(theString[i])) result += " ";
                result += theString[i];
            }

            return result;
        }


        /// <summary>
        ///     Calculates the MD5 hash.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>MD5 hash over the input</returns>
        public static string CalculateMd5Hash(this string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var t in hash) sb.Append(t.ToString("X2"));
            return sb.ToString();
        }

        public static int ToInt32(this string value)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hash = md5.ComputeHash(inputBytes);
            
            // step2, use BitConverter to convert the hash to integer
            return BitConverter.ToInt32(hash);

        }
    }
}