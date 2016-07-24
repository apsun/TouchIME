namespace SynapticsInput
{
    /// <summary>
    /// Definitions of error codes returned by the Synaptics COM API.
    /// </summary>
    internal enum SynError
    {
        Handle = unchecked((int)0x80070006L),
        Fail = unchecked((int)0x80004005L),
        InvalidArg = unchecked((int)0x80070057L),
        OutOfMemory = unchecked((int)0x8007000EL),
        AccessDenied = unchecked((int)0x80070005),
        NotImpl = unchecked((int)0x80004001),
        Sequence = unchecked((int)0x8000FFFFL),
    }
}
