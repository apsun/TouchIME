using System.Collections.Generic;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Event data for a input recognition event.
    /// </summary>
    public struct TouchRecognitionEventArgs
    {
        /// <summary>
        /// Gets the list of recognition results, sorted from
        /// most to least confident. This list may be empty if
        /// there is no input or if no matches were found.
        /// </summary>
        public IReadOnlyList<string> Results { get; }

        /// <summary>
        /// Initializes the event data with the given result list.
        /// </summary>
        /// <param name="results">The list of recognition results.</param>
        public TouchRecognitionEventArgs(IReadOnlyList<string> results)
        {
            Results = results;
        }
    }
}
