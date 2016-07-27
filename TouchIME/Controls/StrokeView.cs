using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TouchIME.Input;

namespace TouchIME.Controls
{
    /// <summary>
    /// Displays touch strokes.
    /// </summary>
    public sealed class StrokeView : Panel
    {
        private float _strokeWidth = 2.0f;
        private IReadOnlyList<TouchStroke> _strokes;

        /// <summary>
        /// Gets or sets the stroke width.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(2.0f)]
        [Description("The width of strokes drawn in the control.")]
        public float StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                if (_strokeWidth != value)
                {
                    _strokeWidth = value;
                    Invalidate();
                }
            }
        }

        public StrokeView()
        {
            DoubleBuffered = true;
        }

        /// <summary>
        /// Updates the list of strokes used to render this control.
        /// The list must remain valid until the next call to
        /// <see cref="UpdateStrokes(IReadOnlyList{TouchStroke})"/>.
        /// </summary>
        public void UpdateStrokes(IReadOnlyList<TouchStroke> strokes)
        {
            _strokes = strokes;
            Invalidate();
        }

        private Point[] TranslatePoints(TouchStroke stroke)
        {
            Rectangle touchArea = stroke.TouchArea;
            Point[] points = new Point[stroke.PointCount];
            for (int i = 0; i < points.Length; ++i)
            {
                Point strokePoint = stroke[i];
                int relX = strokePoint.X - touchArea.X;
                int absX = relX * Width / touchArea.Width;
                int relY = strokePoint.Y - touchArea.Y;
                int absY = relY * Height / touchArea.Height;
                points[i] = new Point(absX, absY);
            }
            return points;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            IReadOnlyList<TouchStroke> strokes = _strokes;
            if (strokes != null)
            {
                using (Pen pen = new Pen(ForeColor, StrokeWidth))
                {
                    foreach (TouchStroke stroke in strokes)
                    {
                        if (stroke.PointCount >= 2)
                        {
                            e.Graphics.DrawLines(pen, TranslatePoints(stroke));
                        }
                    }
                }
            }
        }
    }
}
