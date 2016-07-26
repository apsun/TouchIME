namespace TouchIME.Input.Synaptics
{
    /// <summary>
    /// Definitions of error codes returned by the Synaptics COM API.
    /// </summary>
    internal enum SynError
    {
        Handle = unchecked((int)0x80070006),
        Fail = unchecked((int)0x80004005),
        InvalidArg = unchecked((int)0x80070057),
        OutOfMemory = unchecked((int)0x8007000E),
        AccessDenied = unchecked((int)0x80070005),
        NotImpl = unchecked((int)0x80004001),
        Sequence = unchecked((int)0x8000FFFF),
    }
}
