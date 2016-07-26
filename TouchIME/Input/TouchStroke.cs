using System.Collections.Generic;
using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Represents a single stroke, composed of touch points.
    /// </summary>
    public sealed class TouchStroke
    {
        private readonly List<Point> _points = new List<Point>();

        /// <summary>
        /// Gets a rectangle describing the area (touchable region)
        /// of the input source that generated this stroke. The
        /// coordinate axis is aligned such that (0,0) is located
        /// at the top-left corner.
        /// </summary>
        public Rectangle TouchArea { get; }

        /// <summary>
        /// Gets the number of points in this stroke.
        /// </summary>
        public int PointCount => _points.Count;

        /// <summary>
        /// Gets the specified point in this stroke.
        /// </summary>
        /// <param name="index">The index of the point.</param>
        public Point this[int index] => _points[index];

        /// <summary>
        /// Initializes the stroke with the specified touch area.
        /// </summary>
        /// <param name="area">The bounds for all points in this stroke.</param>
        public TouchStroke(Rectangle area)
        {
            TouchArea = area;
        }

        /// <summary>
        /// Adds a new point to the end of the stroke.
        /// </summary>
        /// <param name="point">The point to add.</param>
        internal void AddPoint(Point point)
        {
            _points.Add(point);
        }

        /// <summary>
        /// Gets an array containing all points in the stroke.
        /// </summary>
        public Point[] GetPoints()
        {
            return _points.ToArray();
        }
    }
}
