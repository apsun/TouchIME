using System.Collections.Generic;
using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Event data for a stroke event. Note that this is a
    /// struct for performance reasons.
    /// </summary>
    public struct TouchStrokeEventArgs
    {
        /// <summary>
        /// The points that make up the stroke. This is returned as a
        /// List for performance reasons; you must not modify it.
        /// </summary>
        public List<Point> Stroke { get; }

        /// <summary>
        /// Initializes the stroke event data with the given point list. 
        /// </summary>
        /// <param name="stroke">The points that make up the stroke.</param>
        public TouchStrokeEventArgs(List<Point> stroke)
        {
            Stroke = stroke;
        }
    }
}
