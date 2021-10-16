using System.Security.Cryptography;
using System.Text;

namespace M5x.Crypto
{
    public class Md5
    {
        public static string Encode(string str)
        {
            var x = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = x.ComputeHash(bs);

            var sb = new StringBuilder();
            foreach (var b in bs) sb.Append(b.ToString("x2").ToLower());

            return sb.ToString();
        }
    }
}