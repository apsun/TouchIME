using System;

namespace SynapticsInput
{
    /// <summary>
    /// Exception thrown if the Synaptics kernel-mode driver
    /// is not installed or is too old to support the Synaptics
    /// COM API.
    /// </summary>
    public sealed class SynDriverException : Exception
    {
        internal SynDriverException() : base("Synaptics driver not found or outdated")
        {
            
        }
    }
}
