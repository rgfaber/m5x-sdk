// ***********************************************************************
// <copyright file="HexUtils.cs" company="macula.io">
//     (c)2018 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Text;

namespace M5x.Crypto
{
    /// <summary>
    ///     Utility class for doing Hex encoding/decoding
    /// </summary>
    public static class HexUtils
    {
        #region Members

        #region Privates

        // Holds the hex characters for a fast lookup.
        /// <summary>
        ///     The he x_ chars
        /// </summary>
        private static readonly char[] HexChars = "0123456789ABCDEF".ToCharArray();

        #endregion


        #region Publics

        // Publics members goes here.

        #endregion

        #endregion

        #region Constructor(s) & Destructor

        //Keep the array sorted for faster search
        /// <summary>
        ///     Initializes static members of the <see cref="HexUtils" /> class.
        /// </summary>
        static HexUtils()
        {
            Array.Sort(HexChars);
        }

        // No explicit client creation needed/allowed

        #endregion

        #region Properties

        // Accessors goes here.

        #endregion

        #region Events

        // Events and delegates goes here.

        #endregion

        #region Methods

        #region Statics

        /// <summary>
        ///     Creates a byte array from the hexadecimal string. Each two characters are combined
        ///     to create one byte. First two hexadecimal characters become first byte in returned array.
        ///     If input hex string contains an odd number of bytes, then last character is dropped to
        ///     make the returned byte array as even bytes. It raises ArgumentException, if a
        ///     Non-hexadecimal character is encountered while processing the string.
        /// </summary>
        /// <param name="hexString">
        ///     String to convert to byte array. It should contain only
        ///     Hex chars [0-9 a-f A-F] only, else error will be raised. See description above
        /// </param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        /// <exception cref="System.ArgumentException">
        ///     Invalid hexString size
        ///     or
        ///     Non Hexadecimal character ' + c + ' in hexString
        ///     or
        ///     hexString must be a valid hexadecimal
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Invalid hexString size
        ///     or
        ///     Non Hexadecimal character ' + c + ' in hexString
        ///     or
        ///     hexString must be a valid hexadecimal
        /// </exception>
        /// <exception cref="ArgumentException">Invalid hexString size</exception>
        /// <exception cref="ArgumentException">Non Hexadecimal character in hexString</exception>
        public static byte[] GetBytesFromHex(string hexString)
        {
            if (!IsValidHexCharLength(hexString)) throw new ArgumentException("Invalid hexString size");

            // Check for non hex characters
            var tempString = new StringBuilder(hexString.Length);
            foreach (var c in hexString)
                if (IsHexChar(c))
                    tempString.Append(c);
                else
                    throw new ArgumentException("Non Hexadecimal character '" + c + "' in hexString");

            var verifiedHexString = tempString.ToString();
            tempString = null;

            // Check for valid length. If number of characters is odd in the hex string then
            // drop the last character
            if (verifiedHexString.Length % 2 != 0)
                verifiedHexString = verifiedHexString.Substring(0, verifiedHexString.Length - 1);


            // Convert each hex character to byte
            // Hex byte length is half of actual ascii byte length
            var byteArrayLength = verifiedHexString.Length / 2;
            var hexbytes = new byte[byteArrayLength];
            string tmpSubstring = null;
            try
            {
                for (var i = 0; i < hexbytes.Length; i++)
                {
                    var charIndex = i * 2;
                    tmpSubstring = verifiedHexString.Substring(charIndex, 2);
                    hexbytes[i] = Convert.ToByte(tmpSubstring, 16);
                }
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("hexString must be a valid hexadecimal", ex);
            }

            return hexbytes;
        }

        /// <summary>
        ///     Convert an hexadecimal string to an ASCII String.
        /// </summary>
        /// <param name="hexString">
        ///     String to convert to byte array. It should contain only
        ///     Hex chars [0-9 a-f A-F] only, else error will be raised. See description above
        /// </param>
        /// <returns>hax values for all characters from the original string</returns>
        /// <example>
        ///     <code>
        /// string str = HexUtils.GetStringFromHex("4145");
        /// </code>
        /// </example>
        public static string GetStringFromHex(string hexString)
        {
            var ch = GetBytesFromHex(hexString);
            return Encoding.ASCII.GetString(ch);
        }


        /// <summary>
        ///     Converts the input bytes array to a String assuming each character as a hexadecimal
        ///     character. Uses .NET Byte.ToString("X2") implementation to achieve the encoding
        /// </summary>
        /// <param name="bytes">hex encoded bytes to be converted to the string</param>
        /// <returns>A string representation of the input bytes assuming Hex encoding</returns>
        /// <exception cref="System.ArgumentNullException">bytes</exception>
        /// <exception cref="ArgumentNullException">bytes</exception>
        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("bytes");

            var hexString = new StringBuilder();

            foreach (var byt in bytes) hexString.Append(byt.ToString("X2"));

            return hexString.ToString();
        }


        /// <summary>
        ///     Convert a string composed by the hexadecimal codes of each characters (ASCII) from the
        ///     parameters string.
        /// </summary>
        /// <param name="clearString">String to process</param>
        /// <returns>Hex codes of characters</returns>
        /// <example>
        ///     <code>
        /// string hexStr = HexUtils.ToString("AE");
        /// </code>
        /// </example>
        public static string ToHexString(string clearString)
        {
            var ab = Encoding.ASCII.GetBytes(clearString);
            return ToHexString(ab);
        }

        /// <summary>
        ///     Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString">string to be tested for valid hexadecimal content</param>
        /// <returns>True if hexString contains only hex characters and at least two characters long else False.</returns>
        public static bool IsHexString(string hexString)
        {
            var hexFormat = IsValidHexCharLength(hexString);
            if (hexFormat)
                foreach (var ch in hexString)
                    if (!IsHexChar(ch))
                    {
                        hexFormat = false;
                        break;
                    }

            return hexFormat;
        }

        /// <summary>
        ///     Checks to see if the input string is not null and atleast two
        ///     characters long
        /// </summary>
        /// <param name="hexString">string to be tested</param>
        /// <returns>
        ///     true if hexString is not null and atleast two
        ///     characters long
        /// </returns>
        private static bool IsValidHexCharLength(string hexString)
        {
            return hexString != null && hexString.Length >= 2;
        }

        /// <summary>
        ///     Returns true if c is a hexadecimal character [A-F, a-f, 0-9]
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if c is a hex char, false if not</returns>
        public static bool IsHexChar(char c)
        {
            c = char.ToUpper(c);

            // Look-up the char in HEX_CHARS Array
            return Array.BinarySearch(HexChars, c) >= 0;
        }

        /// <summary>
        ///     This is a utility method to convert any input byte[] to the requiredSize. If the input
        ///     byte[] is smaller than requiredSize, then a new byte array is created where all lower order
        ///     places are filled with input byte[] and balance places are filled with 0 (zeros). inputBytes
        ///     array is truncated if it is larger than requiredSize.
        /// </summary>
        /// <param name="inputBytes">input byte aray</param>
        /// <param name="requiredSize">the size of the byte array to be created</param>
        /// <returns>a byte array of requiredSize</returns>
        public static byte[] CreateLegalByteArray(byte[] inputBytes, int requiredSize)
        {
            byte[] newBytes = null;
            var inputLength = inputBytes.Length;

            if (inputLength == requiredSize)
            {
                // Nothing to do
                newBytes = inputBytes;
            }
            else
            {
                // Create a new Byte array of reuired lenght and fill the content with
                // given byte[] starting from 0 index in the new byte[]
                newBytes = new byte[requiredSize];
                var len = newBytes.Length;
                if (len > inputLength) len = inputLength;

                Array.Copy(inputBytes, newBytes, len); //note: balance is filled with 0 (zero)
            }

            return newBytes;
        }

        #endregion

        #endregion
    }
}