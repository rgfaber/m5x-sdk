using System.Security.Cryptography;
using System.Text;

namespace M5x.Crypto
{
    public static class Md5
    {
        public static string MD5Encode(this string str)
        {
            var x = MD5.Create(); 
            var bs = Encoding.UTF8.GetBytes(str);
            bs = x.ComputeHash(bs);

            var sb = new StringBuilder();
            foreach (var b in bs) sb.Append(b.ToString("x2").ToLower());

            return sb.ToString();
        }
    }
}