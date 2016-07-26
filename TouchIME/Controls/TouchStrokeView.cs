using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TouchIME.Input;

namespace TouchIME.Controls
{
    /// <summary>
    /// Displays touch stroke input.
    /// </summary>
    public sealed partial class TouchStrokeView : Panel
    {
        private TouchAdapter _adapter;
        private float _strokeWidth = 2.0f;

        /// <summary>
        /// Gets or sets the width of 
        /// </summary>
        [Browsable(true)]
        [DefaultValue(2.0f)]
        [Description("The width used to draw strokes in the control.")]
        public float StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                if (_strokeWidth == value) return;
                _strokeWidth = value;
                Invalidate();
            }
        }

        public TouchAdapter Adapter
        {
            get { return _adapter; }
            set
            {
                TouchAdapter oldAdapter = _adapter;
                if (oldAdapter == value) return;
                if (oldAdapter != null)
                {
                    oldAdapter.StrokeChanged -= OnStrokesChanged;
                    oldAdapter.StrokesCleared -= OnStrokesChanged;
                }
                if (value != null)
                {
                    value.StrokeChanged += OnStrokesChanged;
                    value.StrokesCleared += OnStrokesChanged;
                }
                _adapter = value;
                Invalidate();
            }
        }

        public TouchStrokeView()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void OnStrokesChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private Point[] TranslatePoints(List<Point> stroke)
        {
            Rectangle touchArea = _adapter.TouchArea;
            Point[] points = new Point[stroke.Count];
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
            if (_adapter != null)
            {
                using (Pen pen = new Pen(ForeColor, StrokeWidth))
                {
                    foreach (List<Point> stroke in _adapter.Strokes)
                    {
                        if (stroke.Count >= 2)
                        {
                            e.Graphics.DrawLines(pen, TranslatePoints(stroke));
                        }
                    }
                }
            }
        }
    }
}
