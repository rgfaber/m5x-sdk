using System.Text;

namespace M5x.Schemas.Extensions
{
    public static class IdentityExtensions
    {
        public static byte[] GetBytes(this IIdentity identity)
        {
            return Encoding.UTF8.GetBytes(identity.Value);
        }
    }
}