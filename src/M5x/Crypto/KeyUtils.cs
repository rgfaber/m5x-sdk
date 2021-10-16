using System.Security.Cryptography;
using System.Text;

namespace M5x.Crypto
{
    public static class KeyUtils
    {
        private static string GetSha256Hash(SHA256 shaHash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            var data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string GetKeyFrom(string source)
        {
            return GetSha256Hash(SHA256.Create(), source);
        }
    }
}