using System;
using System.Runtime.InteropServices;

namespace TouchIME.Windows
{
    /// <summary>
    /// Sends keystrokes to other applications using the
    /// <code>SendInput</code> Windows API.
    /// </summary>
    public static class InputHelper
    {
        private enum InputType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2,
        }

        [Flags]
        private enum MouseInputFlags : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            HWheel = 0x1000,
            MoveNoCoalesce = 0x2000,
            VirtualDesk = 0x4000,
            Absolute = 0x8000,
        }

        [Flags]
        private enum KeyboardInputFlags : uint
        {
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            ScanCode = 0x0008,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseInputFlags flags;
            public uint time;
            public UIntPtr extraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort virtualKeyCode;
            public ushort scanCode;
            public KeyboardInputFlags flags;
            public uint time;
            public UIntPtr extraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public uint message;
            public ushort paramLow;
            public ushort paramHigh;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Input
        {
            [FieldOffset(0)] public InputType type;
            [FieldOffset(4)] public MouseInput mouse;
            [FieldOffset(4)] public KeyboardInput keyboard;
            [FieldOffset(4)] public HardwareInput hardware;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint num, Input[] inputs, int size);

        private static readonly int InputSize = Marshal.SizeOf(typeof(Input));

        /// <summary>
        /// Moves the cursor to the specified coordinates. The coordinates
        /// must be between 0 (left/top) and 65535 (bottom/right).
        /// </summary>
        /// <param name="normalizedX">The normalized X coordinates.</param>
        /// <param name="normalizedY">The normalized Y coordinates.</param>
        public static void SendMouseMove(ushort normalizedX, ushort normalizedY)
        {
            Input[] inputs = new Input[1];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.dx = normalizedX;
            inputs[0].mouse.dy = normalizedY;
            inputs[0].mouse.flags = MouseInputFlags.Move | MouseInputFlags.Absolute;
            SendInput(1, inputs, InputSize);
        }

        /// <summary>
        /// Simulates a mouse left click.
        /// </summary>
        public static void SendMouseLeftClick()
        {
            Input[] inputs = new Input[2];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.flags = MouseInputFlags.LeftDown;
            inputs[1].type = InputType.Mouse;
            inputs[1].mouse.flags = MouseInputFlags.LeftUp;
            SendInput(2, inputs, InputSize);
        }

        /// <summary>
        /// Simulates a mouse right click.
        /// </summary>
        public static void SendMouseRightClick()
        {
            Input[] inputs = new Input[2];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.flags = MouseInputFlags.RightDown;
            inputs[1].type = InputType.Mouse;
            inputs[1].mouse.flags = MouseInputFlags.RightUp;
            SendInput(2, inputs, InputSize);
        }

        /// <summary>
        /// Simulates a single keystroke (down and up).
        /// </summary>
        /// <param name="key">The key to press.</param>
        public static void SendKeyboardKey(Key key)
        {
            Input[] inputs = new Input[2];
            inputs[0].type = InputType.Keyboard;
            inputs[0].keyboard.virtualKeyCode = (ushort)key;
            inputs[1].type = InputType.Keyboard;
            inputs[1].keyboard.virtualKeyCode = (ushort)key;
            inputs[1].keyboard.flags = KeyboardInputFlags.KeyUp;
            SendInput(2, inputs, InputSize);
        }

        /// <summary>
        /// Sends the specified text by simulating multiple keystrokes.
        /// </summary>
        /// <param name="text">The text to enter.</param>
        public static void SendKeyboardText(string text)
        {
            Input[] inputs = new Input[text.Length * 2];
            for (int i = 0; i < text.Length; ++i)
            {
                inputs[i * 2].type = InputType.Keyboard;
                inputs[i * 2].keyboard.scanCode = text[i];
                inputs[i * 2].keyboard.flags = KeyboardInputFlags.Unicode;
                inputs[i * 2 + 1].type = InputType.Keyboard;
                inputs[i * 2 + 1].keyboard.scanCode = text[i];
                inputs[i * 2 + 1].keyboard.flags = KeyboardInputFlags.Unicode | KeyboardInputFlags.KeyUp;
            }
            SendInput((uint)inputs.Length, inputs, InputSize);
        }
    }
}
