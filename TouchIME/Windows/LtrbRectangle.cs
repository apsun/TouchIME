using System.Drawing;
using System.Runtime.InteropServices;

namespace TouchIME.Windows
{
    /// <summary>
    /// Managed equivalent of the Win32 <code>RECT</code> structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LtrbRectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        /// <summary>
        /// Creates a new LTRB rectangle.
        /// </summary>
        /// <param name="left">The left coordinate of the rectangle.</param>
        /// <param name="top">The top coordinate of the rectangle.</param>
        /// <param name="right">The right coordinate of the rectangle.</param>
        /// <param name="bottom">The bottom coordinate of the rectangle.</param>
        public LtrbRectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Converts the LTRB rectangle to a XYWH <see cref="Rectangle"/>. 
        /// </summary>
        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(Left, Top, Right, Bottom);
        }

        /// <summary>
        /// Creates a LTRB rectangle from a XYWH <see cref="Rectangle"/>. 
        /// </summary>
        /// <param name="rect">The XYWH rectangle instance.</param>
        public static LtrbRectangle FromRectangle(Rectangle rect)
        {
            return new LtrbRectangle(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
        }

        public override string ToString()
        {
            return "{Left=" + Left + ",Top=" + Top + ",Right=" + Right + ",Bottom=" + Bottom + "}";
        }
    }
}
