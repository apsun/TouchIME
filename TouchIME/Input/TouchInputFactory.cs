using System;
using TouchIME.Input.Synaptics;

namespace TouchIME.Input
{
    /// <summary>
    /// Creates touch input devices.
    /// </summary>
    public static class TouchInputFactory
    {
        private static IRawTouchInput CreateInput(RawTouchInputSource source)
        {
            switch (source)
            {
                case RawTouchInputSource.Synaptics:
                    return new SynTouchpadInput();
                default:
                    throw new ArgumentException("Invalid input source: " + source);
            }
        }

        /// <summary>
        /// Creates an input device with the specified source.
        /// </summary>
        /// <param name="source">The input source.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the input source is not supported/installed on the current system.
        /// </exception>
        public static TouchInput Create(RawTouchInputSource source)
        {
            return new TouchInput(CreateInput(source));
        }
    }
}
