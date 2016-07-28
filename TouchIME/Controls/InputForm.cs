using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
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
        private List<string> _recognizerResults;

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
            
            _recognizer = new StrokeRecognizer();
            StrokeRecognizerEngine[] engines = _recognizer.GetAllEngines();
            comboBox1.Items.AddRange(engines);
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedItem = _recognizer.GetDefaultEngine();
            SystemEvents.PowerModeChanged += OnPowerModeChanged;

            _keyboardManager = new KeyboardManager();
            _keyboardManager.InstallHook();
            _keyboardManager.GlobalKeyDown += _keyboardManager_GlobalKeyDown;
        }

        private void _keyboardManager_GlobalKeyDown(object sender, GlobalKeyEventArgs e)
        {
            if (_recognizerResults == null) return;
            if (KeyboardManager.GetModifierState() != Modifiers.None) return;
            if (e.KeyCode >= Key.D0 && e.KeyCode <= Key.D9)
            {
                int numericValue = ((int)e.KeyCode - (int)Key.D0 + 9) % 10;
                if (numericValue < _recognizerResults.Count)
                {
                    InputRecognitionResult(numericValue);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Key.Space && _recognizerResults.Count > 0)
            {
                InputRecognitionResult(0);
                e.Handled = true;
            }
            else if (e.KeyCode == Key.Backspace && _recognizerResults.Count > 0)
            {
                ClearStrokes();
                e.Handled = true;
            }
        }

        private void InputRecognitionResult(int index)
        {
            InputHelper.SendKeyboardText(_recognizerResults[index]);
            ClearStrokes();
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
            _recognizerResults = null;
            _input.ClearStrokes();
            _recognizer.ClearStrokes();
            UpdateStrokeRecognition();
            strokeDisplay.UpdateStrokes(_input.Strokes);
        }

        private void OnStrokeFinished(object sender, TouchStrokeEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                _recognizer.AddStroke(e.Stroke);
                UpdateStrokeRecognition();
            }));
        }

        private void UpdateStrokeRecognition()
        {
            try
            {
                _recognizerResults = _recognizer.Recognize();
            }
            catch (StrokeRecognitionException ex)
            {
                Debug.WriteLine("Failed to recognize strokes:");
                Debug.WriteLine(ex);
                return;
            }
            object[] resultDisplay = new object[_recognizerResults.Count];
            for (int i = 0; i < resultDisplay.Length; ++i)
            {
                resultDisplay[i] = ((i + 1) % 10) + ". " + _recognizerResults[i];
            }
            listBox1.Items.Clear();
            listBox1.Items.AddRange(resultDisplay);
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _input.StopTouchCapture();
            _input.TouchEnabled = false;
            _input.Dispose();
            _input = null;
            _keyboardManager.UninstallHook();
            _recognizer.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearStrokes();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _recognizer.SetEngine(((StrokeRecognizerEngine)comboBox1.SelectedItem).Id);
            UpdateStrokeRecognition();
        }
    }
}
