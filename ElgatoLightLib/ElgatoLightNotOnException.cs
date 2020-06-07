using System;
using System.Runtime.Serialization;

namespace ElgatoLightLib
{
    [Serializable]
    internal class ElgatoLightNotOnException : Exception
    {
        public ElgatoLightNotOnException()
        {
        }

        public ElgatoLightNotOnException(string message) : base(message)
        {
        }

        public ElgatoLightNotOnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElgatoLightNotOnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}