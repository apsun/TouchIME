using System;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Exception thrown if stroke recognition fails for an
    /// unknown reason.
    /// </summary>
    public class StrokeRecognitionException : Exception
    {
        /// <summary>
        /// Creates a new instance of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StrokeRecognitionException(string message) : base(message)
        {

        }
    }
}
