using System.Collections.Generic;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Event data for a input recognition event. Note that this is a
    /// struct for performance reasons.
    /// </summary>
    public struct TouchRecognitionEventArgs
    {
        /// <summary>
        /// Gets the list of recognition results, sorted from
        /// most to least confident. This list may be empty if
        /// there is no input or if no matches were found.
        /// This is returned as a List for performance reasons;
        /// you must not modify it.
        /// </summary>
        public List<string> Results { get; }

        /// <summary>
        /// Initializes the event data with the given result list.
        /// </summary>
        /// <param name="results">The list of recognition results.</param>
        public TouchRecognitionEventArgs(List<string> results)
        {
            Results = results;
        }
    }
}
