using System;
using System.Drawing;

namespace TouchInputAPI
{
    public interface ITouchInput : IDisposable
    {
        /// <summary>
        /// Event raised when the user begins touching the device. This
        /// is only raised after calling <see cref="BeginTouchCapture"/>
        /// and before calling <see cref="EndTouchCapture"/>.
        /// </summary>
        event EventHandler TouchStarted;

        /// <summary>
        /// Event raised when the position of the touch changes.
        /// This is only raised after calling <see cref="BeginTouchCapture"/>
        /// and before calling <see cref="EndTouchCapture"/>. 
        /// </summary>
        event EventHandler<TouchEventArgs> TouchMoved;

        /// <summary>
        /// Event raised when the user stops touching the device. This
        /// is only raised after calling <see cref="BeginTouchCapture"/>
        /// and before calling <see cref="EndTouchCapture"/>.
        /// </summary>
        event EventHandler TouchEnded;

        /// <summary>
        /// Gets a rectangle describing the area (touchable region)
        /// of the input source. The coordinate axis is aligned such
        /// that (0,0) is located at the top-left corner.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the object has been disposed.
        /// </exception>
        Rectangle TouchArea { get; }

        /// <summary>
        /// Begins touch capturing. This blocks all normal input from
        /// the device so that click/cursor movement events are not
        /// sent to the system.
        /// </summary>
        /// <exception cref="TouchCaptureException">
        /// Thrown if touch input could not be captured.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the object has been disposed.
        /// </exception>
        void BeginTouchCapture();

        /// <summary>
        /// Ends touch capturing. This restores normal cursor behavior
        /// to the system.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the object has been disposed.
        /// </exception>
        void EndTouchCapture();
    }
}
