using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TouchIME.Input;

namespace TouchIME.Controls
{
    public sealed partial class TouchStrokeView : Control
    {
        private TouchStrokeAdapter _strokeAdapter;

        public TouchStrokeAdapter StrokeAdapter
        {
            get { return _strokeAdapter; }
            set
            {
                TouchStrokeAdapter oldAdapter = _strokeAdapter;
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
                _strokeAdapter = value;
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
            Rectangle touchArea = _strokeAdapter.TouchArea;
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
            if (_strokeAdapter != null)
            {
                foreach (List<Point> stroke in _strokeAdapter.Strokes)
                {
                    if (stroke.Count >= 2)
                    {
                        e.Graphics.DrawLines(Pens.Red, TranslatePoints(stroke));
                    }
                }
            }
        }
    }
}
