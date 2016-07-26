using System;
using System.Windows.Forms;
using Microsoft.Ink;
using TouchIME.Input;
using TouchIME.Input.Synaptics;
using TouchIME.Recognition;
using TouchIME.Windows;

namespace TouchIME
{
    public partial class Form1 : Form
    {
        private ITouchInput _touchInput;
        private TouchAdapter _touchAdapter;
        private HotkeyManager _hotkeyManager;
        private TouchRecognizer _recognizer;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _touchInput = TouchInputFactory.CreateInput();
            _touchInput.StartTouchCapture();
            _touchInput.TouchMoved += _touchInput_TouchMoved;
            ((SynTouchpadInput)_touchInput).TouchEnabled = true;

            _touchAdapter = new TouchAdapter
            {
                Input = _touchInput,
                StrokeArea = _touchInput.TouchArea
            };

            _recognizer = new TouchRecognizer();
            _recognizer.SetAdapter(_touchAdapter);
            Recognizers re = new Recognizers();
            _recognizer.SetRecognizer(re[1]);
            _recognizer.ResultsChanged += RecognizerResultsChanged;

            this.touchStrokeView1.Adapter = _touchAdapter;
        }

        private void _touchInput_TouchMoved(object sender, TouchEventArgs e)
        {
            /*label1.Invoke(new Action(() =>
            {
                label1.Text = e.Location.ToString();
            }));*/
        }

        private void RecognizerResultsChanged(object sender, TouchRecognitionEventArgs e)
        {
            label1.Invoke(new Action(() =>
            {
                if (e.Results.Count > 0)
                {
                    label1.Text = e.Results[0];
                }
                else
                {
                    label1.Text = "No results";
                }
            }));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _touchInput.StopTouchCapture();
            ((SynTouchpadInput)_touchInput).TouchEnabled = false;
            _touchInput.Dispose();
            _touchInput = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _touchAdapter.Clear();
        }

        protected override void WndProc(ref Message m)
        {
            _hotkeyManager?.HandleWndProc(ref m);
            base.WndProc(ref m);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
