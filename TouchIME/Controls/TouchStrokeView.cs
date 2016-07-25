using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TouchIME.Input;

namespace TouchIME.Controls
{
    public partial class TouchStrokeView : Control
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
        }

        private void OnStrokesChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
