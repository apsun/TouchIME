using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Ink;
using TouchIME.Input;
using TouchIME.Recognition;

namespace TouchIME.Controls
{
    public partial class InputForm : Form
    {
        private TouchInput _input;
        private StrokeRecognizer _recognizer;

        public InputForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _input = TouchInputFactory.Create(RawTouchInputType.Synaptics);
            _input.StartTouchCapture();
            _input.TouchEnabled = true;
            _input.StrokeChanged += OnStrokeChanged;
            _input.StrokeFinished += OnStrokeFinished;

            Recognizer[] res = RecognizerHelper.GetAll();
            comboBox1.Items.AddRange(res);
            comboBox1.DisplayMember = "Name";
            _recognizer = new StrokeRecognizer();
            _recognizer.SetRecognizer(RecognizerHelper.GetDefault());
        }

        private void OnStrokeChanged(object sender, EventArgs e)
        {
            touchStrokeView1.UpdateStrokes(_input.Strokes);
        }

        private void ClearStrokes()
        {
            _input.ClearStrokes();
            _recognizer.ClearStrokes();
            UpdateStrokeRecognition();
            touchStrokeView1.UpdateStrokes(_input.Strokes);
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
