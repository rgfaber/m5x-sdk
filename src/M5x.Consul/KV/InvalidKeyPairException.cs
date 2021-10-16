using System;

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
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}