using System;
using System.Drawing;

namespace TouchIME.Input
{
    /// <summary>
    /// Interface to a touch input source.
    /// </summary>
    public interface IRawTouchInput : IDisposable
    {
        /// <summary>
        /// Event raised when the user begins touching the device. This
        /// is only raised after calling <see cref="StartTouchCapture"/>
        /// and before calling <see cref="StopTouchCapture"/>.
        /// </summary>
        event EventHandler TouchStarted;

        /// <summary>
        /// Event raised when the position of the touch changes.
        /// This is only raised after calling <see cref="StartTouchCapture"/>
        /// and before calling <see cref="StopTouchCapture"/>. 
        /// </summary>
        event EventHandler<RawTouchEventArgs> TouchMoved;

        /// <summary>
        /// Event raised when the user stops touching the device. This
        /// is only raised after calling <see cref="StartTouchCapture"/>
        /// and before calling <see cref="StopTouchCapture"/>.
        /// </summary>
        event EventHandler TouchEnded;

        /// <summary>
        /// Gets a rectangle describing the area (touchable region)
        /// of the input source. The coordinate axis is aligned such
        /// that (0,0) is located at the top-left corner.
        /// </summary>
        Rectangle TouchArea { get; }

        /// <summary>
        /// Gets or sets whether the device responds to input.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if the device cannot be enabled/disabled.
        /// </exception>
        bool TouchEnabled { get; set; }

        /// <summary>
        /// Begins touch capturing. This blocks all normal input from
        /// the device so that click/cursor movement events are not
        /// sent to the system.
        /// </summary>
        /// <exception cref="TouchCaptureException">
        /// Thrown if touch input could not be captured.
        /// </exception>
        void StartTouchCapture();

        /// <summary>
        /// Ends touch capturing. This restores normal cursor behavior
        /// to the system.
        /// </summary>
        void StopTouchCapture();
    }
}
