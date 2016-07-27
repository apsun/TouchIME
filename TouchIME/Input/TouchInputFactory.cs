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
        /// Thrown if the input device could not be initialized.
        /// </exception>
        public static TouchInput Create(RawTouchInputSource source)
        {
            try
            {
                return new TouchInput(CreateInput(source));
            }
            catch (Exception ex)
            {
                throw new NotSupportedException("Failed to create input device", ex);
            }
        }
    }
}
