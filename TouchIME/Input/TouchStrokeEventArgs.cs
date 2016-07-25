using System.Collections.Generic;
using System.Drawing;

namespace TouchIME.Input
{
    public struct TouchStrokeEventArgs
    {
        public List<Point> Stroke { get; }

        public TouchStrokeEventArgs(List<Point> stroke)
        {
            Stroke = stroke;
        }
    }
}
