using System;
using System.Runtime.Serialization;

namespace eelightlib
{
    [Serializable]
    internal class LightNotOnException : Exception
    {
        public LightNotOnException()
        {
        }

        public LightNotOnException(string message) : base(message)
        {
        }

        public LightNotOnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LightNotOnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}