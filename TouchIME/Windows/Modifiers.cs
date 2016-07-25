using System;

namespace TouchIME.Windows
{
    /// <summary>
    /// Hotkey modifier keys used by <see cref="HotkeyManager"/>.  
    /// </summary>
    [Flags]
    public enum Modifiers : uint
    {
        None = 0x0000,
        Alt = 0x0001,
        Control = 0x0002,
        Shift = 0x0004,
        Win = 0x0008,
        NoRepeat = 0x4000,
    }
}
