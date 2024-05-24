using System;

namespace ElgatoLightLib
{
    /// <summary>
    /// ElgatoLightOutOfRangeException, is a custom exception class that extends the ArgumentOutOfRangeException 
    /// class from the .NET Framework. It is used to handle exceptions related to values that are outside the 
    /// acceptable range for Elgato lights.
    /// </summary>
    [Serializable]
    public class ElgatoLightOutOfRangeException : ArgumentOutOfRangeException
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ElgatoLightOutOfRangeException()
        {
        }

        /// <summary>
        /// ElgatoLightOutOfRangeException Constructor that override the default message.
        /// </summary>
        /// <param name="message">Message to pass into the Exception.</param>
        public ElgatoLightOutOfRangeException(string message) : base(message)
        {
        }

        /// <summary>
        /// ElgatoLightOutOfRangeException Constructor that override the default message and exception.
        /// </summary>
        /// <param name="message">Message to pass into the Exception.</param>
        /// <param name="innerException">The innerException to pass into the base class.</param>
        public ElgatoLightOutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}