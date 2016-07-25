using System;
using System.Collections.Generic;
using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Adapts raw touch input into "strokes". Also serves as a single
    /// point of mutability to simplify hot-swapping of input soures.
    /// </summary>
    public sealed class TouchStrokeAdapter
    {
        private ITouchInput _input;

        /// <summary>
        /// Gets or sets the input source of the adapter.
        /// </summary>
        public ITouchInput Input
        {
            get { return _input; }
            set
            {
                ITouchInput oldInput = _input;
                if (oldInput == value) return;
                if (oldInput != null)
                {
                    oldInput.TouchStarted -= OnTouchStarted;
                    oldInput.TouchMoved -= OnTouchMoved;
                    oldInput.TouchEnded -= OnTouchEnded;
                }
                if (value != null)
                {
                    value.TouchStarted += OnTouchStarted;
                    value.TouchMoved += OnTouchMoved;
                    value.TouchEnded += OnTouchEnded;
                }
                _input = value;
                Clear();
            }
        }

        /// <summary>
        /// Gets a rectangle describing the area (touchable region)
        /// of the input source. The coordinate axis is aligned such
        /// that (0,0) is located at the top-left corner.
        /// </summary>
        public Rectangle TouchArea => Input.TouchArea;

        /// <summary>
        /// Gets a read-only list of all strokes, including
        /// the current incomplete stroke. You must not modify
        /// the returned list.
        /// </summary>
        public List<List<Point>> Strokes { get; } = new List<List<Point>>();

        /// <summary>
        /// Gets the current (incomplete) stroke, or null if
        /// there is no current stroke. You must not modify the
        /// returned list.
        /// </summary>
        public List<Point> CurrentStroke { get; private set; }

        /// <summary>
        /// Event raised when the current stroke is updated.
        /// </summary>
        public event EventHandler StrokeChanged;

        /// <summary>
        /// Event raised when the current stroke is completed.
        /// </summary>
        public event EventHandler<TouchStrokeEventArgs> StrokeFinished;

        /// <summary>
        /// Event raised when all strokes have been cleared.
        /// </summary>
        public event EventHandler StrokesCleared;

        /// <summary>
        /// Clears all stored strokes.
        /// </summary>
        public void Clear()
        {
            Strokes.Clear();
            CurrentStroke = null;
            StrokesCleared?.Invoke(this, EventArgs.Empty);
        }

        private void OnTouchStarted(object sender, EventArgs e)
        {
            CurrentStroke = new List<Point>();
            Strokes.Add(CurrentStroke);
        }
        
        private void OnTouchMoved(object sender, TouchEventArgs e)
        {
            CurrentStroke.Add(e.Location);
            StrokeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTouchEnded(object sender, EventArgs e)
        {
            List<Point> stroke = CurrentStroke;
            CurrentStroke = null;
            StrokeFinished?.Invoke(this, new TouchStrokeEventArgs(stroke));
        }
    }
}
