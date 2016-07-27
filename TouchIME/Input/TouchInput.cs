using System;
using System.Collections.Generic;
using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Converts raw touch input into stroke data.
    /// </summary>
    public sealed class TouchInput : IDisposable
    {
        private readonly IRawTouchInput _input;
        private readonly List<TouchStroke> _strokes = new List<TouchStroke>();

        /// <summary>
        /// Gets a rectangle describing the entire area (touchable region)
        /// of the input source. The coordinate axis is aligned such
        /// that (0,0) is located at the top-left corner.
        /// </summary>
        public Rectangle TouchArea => _input.TouchArea;

        /// <summary>
        /// Gets or sets whether the device responds to input.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if the device cannot be enabled/disabled.
        /// </exception>
        public bool TouchEnabled
        {
            get { return _input.TouchEnabled; }
            set { _input.TouchEnabled = value; }
        }

        /// <summary>
        /// Gets a list of all strokes, including the current
        /// incomplete stroke.
        /// </summary>
        public IReadOnlyList<TouchStroke> Strokes => _strokes;

        /// <summary>
        /// Gets the current (incomplete) stroke, or null if
        /// there is no current stroke.
        /// </summary>
        public TouchStroke CurrentStroke { get; private set; }

        /// <summary>
        /// Event raised when the current stroke is updated.
        /// </summary>
        public event EventHandler StrokeChanged;

        /// <summary>
        /// Event raised when the current stroke is completed.
        /// </summary>
        public event EventHandler<TouchStrokeEventArgs> StrokeFinished;

        internal TouchInput(IRawTouchInput input)
        {
            _input = input;
            input.TouchStarted += OnTouchStarted;
            input.TouchMoved += OnTouchMoved;
            input.TouchEnded += OnTouchEnded;
        }
        
        /// <summary>
        /// Begins touch capturing. This blocks all normal input from
        /// the device so that click/cursor movement events are not
        /// sent to the system.
        /// </summary>
        /// <exception cref="TouchCaptureException">
        /// Thrown if touch input could not be captured.
        /// </exception>
        public void StartTouchCapture()
        {
            _input.StartTouchCapture();
        }

        /// <summary>
        /// Ends touch capturing. This restores normal cursor behavior
        /// to the system.
        /// </summary>
        public void StopTouchCapture()
        {
            _input.StopTouchCapture();
        }

        /// <summary>
        /// Clears all stored strokes.
        /// </summary>
        public void ClearStrokes()
        {
            CurrentStroke = null;
            _strokes.Clear();
        }

        private void OnTouchStarted(object sender, EventArgs e)
        {
            CurrentStroke = new TouchStroke(TouchArea);
            _strokes.Add(CurrentStroke);
        }
        
        private void OnTouchMoved(object sender, RawTouchEventArgs e)
        {
            TouchStroke stroke = CurrentStroke;
            if (stroke != null)
            {
                stroke.AddPoint(e.Location);
                StrokeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTouchEnded(object sender, EventArgs e)
        {
            TouchStroke stroke = CurrentStroke;
            CurrentStroke = null;
            if (stroke != null)
            {
                StrokeFinished?.Invoke(this, new TouchStrokeEventArgs(stroke));
            }
        }

        public void Dispose()
        {
            _input.TouchStarted -= OnTouchStarted;
            _input.TouchMoved -= OnTouchMoved;
            _input.TouchEnded -= OnTouchEnded;
            _input.Dispose();
        }
    }
}
