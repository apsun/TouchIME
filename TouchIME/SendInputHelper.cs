using System;
using System.Runtime.InteropServices;

namespace TouchIME
{
    public static class SendInputHelper
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

        public static void SendMouseMove(ushort normalizedX, ushort normalizedY)
        {
            Input[] inputs = new Input[1];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.dx = normalizedX;
            inputs[0].mouse.dy = normalizedY;
            inputs[0].mouse.flags = MouseInputFlags.Move | MouseInputFlags.Absolute;
            SendInput(1, inputs, Marshal.SizeOf(typeof(Input)));
        }

        public static void SendMouseLeftClick()
        {
            Input[] inputs = new Input[2];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.flags = MouseInputFlags.LeftDown;
            inputs[1].type = InputType.Mouse;
            inputs[1].mouse.flags = MouseInputFlags.LeftUp;
            SendInput(2, inputs, Marshal.SizeOf(typeof(Input)));
        }

        public static void SendMouseRightClick()
        {
            Input[] inputs = new Input[2];
            inputs[0].type = InputType.Mouse;
            inputs[0].mouse.flags = MouseInputFlags.RightDown;
            inputs[1].type = InputType.Mouse;
            inputs[1].mouse.flags = MouseInputFlags.RightUp;
            SendInput(2, inputs, Marshal.SizeOf(typeof(Input)));
        }

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
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
    }
}
