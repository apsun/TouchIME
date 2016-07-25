using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Event data for a touch event. Use <see cref="ITouchInput.TouchArea"/>
    /// to determine how to interpret these coordinates. Note that this is a
    /// struct for performance reasons.
    /// </summary>
    public struct TouchEventArgs
    {
        /// <summary>
        /// X coordinate of the touch event, relative to the
        /// top-left corner.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y coordinate of the touch event, relative to the
        /// top-left corner.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Coordinates of the touch event, relative to the
        /// top-left corner.
        /// </summary>
        public Point Location => new Point(X, Y);

        /// <summary>
        /// Initializes the touch event data with the given X and Y
        /// coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the touch event.</param>
        /// <param name="y">The Y coordinate of the touch event.</param>
        public TouchEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
