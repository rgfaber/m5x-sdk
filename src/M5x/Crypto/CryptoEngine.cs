#region Using directives

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace M5x.Crypto
{
    public partial class CryptoEngine
    {
        /// <summary>
        ///     Encrypts the specified ablock.
        /// </summary>
        /// <param name="ablock">The ablock.</param>
        /// <returns></returns>
        public static string DESEncrypt(string ablock)
        {
            if (string.IsNullOrEmpty(ablock))
                return ablock;

            var des = DES.Create();
            var ms = new MemoryStream();
            var bin = Encoding.UTF8.GetBytes(ablock);

            var cstream = new CryptoStream(ms, des.CreateEncryptor(Key, Iv), CryptoStreamMode.Write);
            cstream.Write(bin, 0, bin.Length);
            cstream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        ///     Decrypts the specified ablock.
        /// </summary>
        /// <param name="ablock">The ablock.</param>
        /// <returns></returns>
        public static string Decrypt(string ablock)
        {
            try
            {
                if (string.IsNullOrEmpty(ablock))
                    return ablock;
                var des = DES.Create();
                var ms = new MemoryStream();
                var bin = Convert.FromBase64String(ablock);

                var cstream = new CryptoStream(ms, des.CreateDecryptor(Key, Iv), CryptoStreamMode.Write);
                cstream.Write(bin, 0, bin.Length);
                cstream.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occurred in Wulka.CryptoEngine.Decrypt(): ", ex);
            }
        }
    }
}