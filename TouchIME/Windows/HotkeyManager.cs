using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TouchIME.Windows
{
    /// <summary>
    /// Manages hotkeys using the <code>RegisterHotKey</code>
    /// Windows API. Note that you must manually unregister
    /// hotkeys once they are no longer needed.
    /// </summary>
    public sealed class HotkeyManager
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr handle, int id, uint modifiers, uint key);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr handle, int id);

        private const int WmHotkey = 0x0312;

        private readonly IntPtr _handle;
        private readonly Dictionary<int, Action> _hotkeys = new Dictionary<int, Action>();
        private int _counter;

        /// <summary>
        /// Creates a new hotkey manager. You must also override
        /// <see cref="Control.WndProc(ref Message)"/> and pass the message to
        /// <see cref="HandleWndProc(ref Message)"/> to handle hotkey messages. 
        /// </summary>
        /// <param name="handle">The handle to receive hotkey messages on.</param>
        public HotkeyManager(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Registers a new global hotkey. This method must be called on
        /// the thread that created the handle passed in the constructor.
        /// </summary>
        /// <param name="modifiers">Modifier keys for the hotkey.</param>
        /// <param name="key">Primary key for the hotkey.</param>
        /// <param name="callback">A callback to run when the hotkey is pressed.</param>
        /// <returns>
        /// The ID of the hotkey, to use with <see cref="Unregister(int)"/>.
        /// </returns>
        public int Register(Modifiers modifiers, Key key, Action callback)
        {
            if (!RegisterHotKey(_handle, _counter, (uint)modifiers, (uint)key))
            {
                throw new Win32Exception();
            }
            _hotkeys.Add(_counter, callback);
            return _counter++;
        }

        /// <summary>
        /// Unregisters a global hotkey created by
        /// <see cref="Register(Modifiers, Key, Action)"/>. This method
        /// must be run on the thread that created the handle passed
        /// in the constructor.
        /// </summary>
        /// <param name="id">The ID of the hotkey.</param>
        public void Unregister(int id)
        {
            if (!_hotkeys.ContainsKey(id))
            {
                throw new ArgumentException("Hotkey with id " + id + " not registered");
            }
            if (!UnregisterHotKey(_handle, id))
            {
                throw new Win32Exception();
            }
            _hotkeys.Remove(id);
        }

        /// <summary>
        /// Unregisters all hotkeys registered by this object.
        /// This method must be run on the thread that created
        /// the handle passed in the constructor.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (int id in _hotkeys.Keys)
            {
                Unregister(id);
            }
            _hotkeys.Clear();
        }

        /// <summary>
        /// Handles hotkey messages posted to the handle passed in
        /// the constructor. You must call this from the handle's
        /// corresponding <see cref="Control.WndProc(ref Message)"/>
        /// method.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>True if a hotkey was handled; false otherwise.</returns>
        public bool HandleWndProc(ref Message message)
        {
            if (message.HWnd != _handle)
            {
                throw new ArgumentException("Message owned by incorrect handle");
            }
            if (message.Msg == WmHotkey)
            {
                int id = message.WParam.ToInt32();
                Action callback;
                if (_hotkeys.TryGetValue(id, out callback))
                {
                    callback();
                    return true;
                }
            }
            return false;
        }
    }
}
