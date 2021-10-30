using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace M5x.Utils
{
    /// <summary>
    ///     Class GuidUtils.
    /// </summary>
    public static class GuidUtils
    {
        public const string TEST_GUID = "7e577e57-7e57-7e57-7e57-7e577e577e57";

        /// <summary>
        ///     Gets the new clean unique identifier.
        /// </summary>
        /// <value>The new clean unique identifier.</value>
        public static string NewCleanGuid => Guid.NewGuid().ToString("N");

        /// <summary>
        ///     Gets the null unique identifier.
        /// </summary>
        /// <value>The null unique identifier.</value>
        public static string NullCleanGuid => Guid.Empty.ToString("N");

        public static string NewGuid => Guid.NewGuid().ToString();
        public static string LowerCaseGuid => NewGuid.ToLowerInvariant();
        
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
        
        public static int ToInt(this Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);
            return bint;
        }
        
        
        public static Guid AnyStringToGuid(this string anyString)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(anyString));
            return new Guid(data);
        }
        
        public static Guid FromDecimal(this decimal value)
        {
            return new Guid(value.ToByteArray());
        }

        
        
        
    }
}