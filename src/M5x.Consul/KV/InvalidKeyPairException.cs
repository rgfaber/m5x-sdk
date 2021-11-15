using System;
using System.Runtime.Serialization;

namespace M5x.Consul.KV
{
    /// <summary>
    ///     Indicates that the key pair data is invalid
    /// </summary>
#if !(CORECLR || PORTABLE || PORTABLE40)
    [Serializable]
#endif
    public class InvalidKeyPairException : Exception
    {
        public InvalidKeyPairException()
        {
        }

        public InvalidKeyPairException(string message) : base(message)
        {
        }

        public InvalidKeyPairException(string message, Exception inner) : base(message, inner)
        {
        }
#if !(CORECLR || PORTABLE || PORTABLE40)
        protected InvalidKeyPairException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}