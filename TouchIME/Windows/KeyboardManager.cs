using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TouchIME.Windows
{
    /// <summary>
    /// Manages global keyboard state using the <code>SetWindowsHookEx</code>
    /// Windows API. Note that you must manually uninstall the hook once
    /// it is no longer needed.
    /// </summary>
    public class KeyboardManager
    {
        private const int WhKeyboardLowLevel = 13;
        private const int WmKeyDown = 0x0100;
        private const int WmKeyUp = 0x0101;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int type, LowLevelKeyboardProc hook, IntPtr module, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);

        /// <summary>
        /// Event raised when a key has been pressed. The hook must
        /// be installed for this event to be raised.
        /// </summary>
        public event EventHandler<GlobalKeyEventArgs> GlobalKeyDown;

        /// <summary>
        /// Event raised when a key has been released. The hook must
        /// be installed for this event to be raised.
        /// </summary>
        public event EventHandler<GlobalKeyEventArgs> GlobalKeyUp;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private readonly LowLevelKeyboardProc _callback;
        private IntPtr _hook;

        /// <summary>
        /// Initializes the keyboard manager. Call <see cref="InstallHook"/>
        /// to begin receiving key press events. 
        /// </summary>
        public KeyboardManager()
        {
            // This is necessary to prevent the callback from being GC'd
            _callback = HookCallback;
        }

        /// <summary>
        /// Installs the global keyboard hook. You must uninstall
        /// the hook with <see cref="UninstallHook"/>. 
        /// </summary>
        public void InstallHook()
        {
            if (_hook != IntPtr.Zero) return;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
                if (moduleHandle == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                IntPtr hook = SetWindowsHookEx(WhKeyboardLowLevel, _callback, moduleHandle, 0);
                if (hook == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                _hook = hook;
            }
        }

        /// <summary>
        /// Uninstalls the global keyboard hook.
        /// </summary>
        public void UninstallHook()
        {
            if (_hook == IntPtr.Zero) return;
            if (!UnhookWindowsHookEx(_hook))
            {
                throw new Win32Exception();
            }
            _hook = IntPtr.Zero;
        }

        /// <summary>
        /// Checks whether the specified key is currently being pressed.
        /// </summary>
        /// <param name="keyCode">The virtual key code of the key.</param>
        public static bool IsKeyDown(Key keyCode)
        {
            return (GetKeyState((int)keyCode) & 0x8000) != 0;
        }

        /// <summary>
        /// Checks whether the specified key is in a toggled state.
        /// </summary>
        /// <param name="keyCode">The virtual key code of the key.</param>
        public static bool IsKeyToggled(Key keyCode)
        {
            return (GetKeyState((int)keyCode) & 0x0001) != 0;
        }

        /// <summary>
        /// Gets the currently pressed modifier keys.
        /// </summary>
        public static Modifiers GetModifierState()
        {
            Modifiers mod = Modifiers.None;
            if (IsKeyDown(Key.Alt)) mod |= Modifiers.Alt;
            if (IsKeyDown(Key.Control)) mod |= Modifiers.Control;
            if (IsKeyDown(Key.Shift)) mod |= Modifiers.Shift;
            if (IsKeyDown(Key.LeftWin) || IsKeyDown(Key.RightWin)) mod |= Modifiers.Win;
            return mod;
        }

        private bool OnGlobalKeyDown(Key keyCode)
        {
            GlobalKeyEventArgs args = new GlobalKeyEventArgs(keyCode);
            GlobalKeyDown?.Invoke(this, args);
            return args.Handled;
        }

        private bool OnGlobalKeyUp(Key keyCode)
        {
            GlobalKeyEventArgs args = new GlobalKeyEventArgs(keyCode);
            GlobalKeyUp?.Invoke(this, args);
            return args.Handled;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Lazy way of reading the key code, works because
                // it's the first member in the KBDLLHOOKSTRUCT structure
                Key keyCode = (Key)Marshal.ReadInt32(lParam);
                Debug.WriteLine("Key: " + keyCode);
                switch (wParam.ToInt32())
                {
                    case WmKeyDown:
                        if (OnGlobalKeyDown(keyCode)) return new IntPtr(1);
                        break;
                    case WmKeyUp:
                        if (OnGlobalKeyUp(keyCode)) return new IntPtr(1);
                        break;
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        ~KeyboardManager()
        {
            if (_hook != IntPtr.Zero)
            {
                Debug.WriteLine("KeyboardManager finalized before uninstalling hook!");
            }
        }
    }
}
