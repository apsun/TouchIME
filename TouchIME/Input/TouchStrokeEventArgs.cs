namespace TouchIME.Input
{
    /// <summary>
    /// Event data for a stroke event. Note that this is a
    /// struct for performance reasons.
    /// </summary>
    public struct TouchStrokeEventArgs
    {
        /// <summary>
        /// Gets the stroke data associated with this event.
        /// </summary>
        public TouchStroke Stroke { get; }

        /// <summary>
        /// Initializes the event data with the given stroke.
        /// </summary>
        /// <param name="stroke">The stroke data.</param>
        public TouchStrokeEventArgs(TouchStroke stroke)
        {
            Stroke = stroke;
        }
    }
}
