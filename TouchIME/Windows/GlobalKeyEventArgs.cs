namespace TouchIME.Windows
{
    /// <summary>
    /// Event data for a global keypress event.
    /// </summary>
    public class GlobalKeyEventArgs
    {
        /// <summary>
        /// The virtual key code of the key that was pressed.
        /// </summary>
        public Key KeyCode { get; }

        /// <summary>
        /// Gets or sets whether to intercept the keypress.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes the event data with the given key code.
        /// </summary>
        /// <param name="keyCode">The virtual key code of the pressed key.</param>
        public GlobalKeyEventArgs(Key keyCode)
        {
            KeyCode = keyCode;
        }
    }
}
