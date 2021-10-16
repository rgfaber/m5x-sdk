using System.Text;
using EventFlow.Core;

namespace M5x.CEQS.Schema.Extensions
{
    public static class IdentityExtensions
    {
        public static byte[] GetBytes(this IIdentity identity)
        {
            return Encoding.UTF8.GetBytes(identity.Value);
        }
    }
}