using System;
using System.Runtime.Serialization;

namespace ElgatoLightLib
{
    [Serializable]
    public class ElgatoLightOutOfRangeException : ArgumentOutOfRangeException
    {
        public ElgatoLightOutOfRangeException()
        {
        }

        public ElgatoLightOutOfRangeException(string message) : base(message)
        {
        }

        public ElgatoLightOutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ElgatoLightOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}