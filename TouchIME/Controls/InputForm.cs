using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Ink;
using Microsoft.Win32;
using TouchIME.Input;
using TouchIME.Recognition;
using TouchIME.Windows;

namespace TouchIME.Controls
{
    public partial class InputForm : AnchoredForm
    {
        private TouchInput _input;
        private StrokeRecognizer _recognizer;
        private KeyboardManager _keyboardManager;

        private const int WsExNoActivate = 0x08000000;

        public InputForm()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Prevents the form from being focused, allowing
                // input to pass through to the next window.
                CreateParams p = base.CreateParams;
                p.ExStyle |= WsExNoActivate;
                return p;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _input = TouchInputFactory.Create(RawTouchInputSource.Synaptics);
            _input.StartTouchCapture();
            _input.TouchEnabled = true;
            _input.StrokeChanged += OnStrokeChanged;
            _input.StrokeFinished += OnStrokeFinished;

            Recognizer[] res = RecognizerHelper.GetAll();
            comboBox1.Items.AddRange(res);
            comboBox1.DisplayMember = "Name";
            _recognizer = new StrokeRecognizer();
            _recognizer.SetRecognizer(RecognizerHelper.GetDefault());
            SystemEvents.PowerModeChanged += OnPowerModeChanged;

            _keyboardManager = new KeyboardManager();
            _keyboardManager.InstallHook();
            _keyboardManager.GlobalKeyDown += _keyboardManager_GlobalKeyDown;
        }

        private void _keyboardManager_GlobalKeyDown(object sender, GlobalKeyEventArgs e)
        {
            if (e.KeyCode == Key.T)
            {
                InputHelper.SendKeyboardText("hello");
                e.Handled = true;
            }
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend && Visible)
            {
                Close();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                _input.StartTouchCapture();
            }
            else
            {
                _input.StopTouchCapture();
            }
        }

        private void OnStrokeChanged(object sender, EventArgs e)
        {
            strokeDisplay.UpdateStrokes(_input.Strokes);
        }

        private void ClearStrokes()
        {
            _input.ClearStrokes();
            _recognizer.ClearStrokes();
            UpdateStrokeRecognition();
            strokeDisplay.UpdateStrokes(_input.Strokes);
        }

        private void OnStrokeFinished(object sender, TouchStrokeEventArgs e)
        {
            _recognizer.AddStroke(e.Stroke);
            UpdateStrokeRecognition();
        }

        private void UpdateStrokeRecognition()
        {
            List<string> results = _recognizer.Recognize();
            listBox1.BeginInvoke(new Action(() =>
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(results.ToArray());
            }));
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _input.StopTouchCapture();
            _input.TouchEnabled = false;
            _input.Dispose();
            _input = null;
            _keyboardManager.UninstallHook();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearStrokes();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _recognizer.SetRecognizer((Recognizer)comboBox1.SelectedItem);
            UpdateStrokeRecognition();
        }
    }
}
