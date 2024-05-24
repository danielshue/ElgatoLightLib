using System;
using System.Runtime.Serialization;

namespace ElgatoLightLib
{
    /// <summary>
    /// ElgatoLightNotOnException is thrown in situations where an Elgato light is not turned on or fails to turn on.
    /// </summary>
    [Serializable]
    internal class ElgatoLightNotOnException : Exception
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ElgatoLightNotOnException()
        {
        }

        /// <summary>
        /// Default Constructor that overrides the default message.
        /// </summary>
        /// <param name="message">Message passed to the exception base.</param>
        public ElgatoLightNotOnException(string message) : base(message)
        {
        }

        /// <summary>
        /// Default Constructor that overrides the default message.
        /// </summary>
        /// <param name="message">Message passed to the exception base.</param>
        /// <param name="innerException">Exception passed to the base exception class.</param>
        public ElgatoLightNotOnException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}