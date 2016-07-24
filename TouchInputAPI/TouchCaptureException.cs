using System;

namespace TouchInputAPI
{
    /// <summary>
    /// Raised when attempting to capture input from a touch source
    /// that does not currently support/allow capturing.
    /// </summary>
    public sealed class TouchCaptureException : Exception
    {
        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        public TouchCaptureException() : this("Could not capture touch input")
        {

        }

        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TouchCaptureException(string message) : this(message, null)
        {

        }

        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        /// <param name="inner">The exception that caused this exception.</param>
        public TouchCaptureException(Exception inner) : this("Could not capture touch input", inner)
        {

        }

        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that caused this exception.</param>
        public TouchCaptureException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
