using System;
using TouchIME.Input.Synaptics;

namespace TouchIME.Input
{
    /// <summary>
    /// Creates touch input devices.
    /// </summary>
    public static class TouchInputFactory
    {
        private static IRawTouchInput CreateInput(RawTouchInputType type)
        {
            switch (type)
            {
                case RawTouchInputType.Synaptics:
                    return new SynTouchpadInput();
                default:
                    throw new ArgumentException("Invalid input device type: " + type);
            }
        }

        /// <summary>
        /// Creates an input device of the specified type.
        /// </summary>
        /// <param name="type">The input device type.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the device is not supported/installed on the current system.
        /// </exception>
        public static TouchInput Create(RawTouchInputType type)
        {
            return new TouchInput(CreateInput(type));
        }
    }
}
