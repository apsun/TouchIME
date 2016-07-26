using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Ink;
using TouchIME.Input;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Dynamically converts touch input to text. Use the
    /// <see cref="ResultsChanged"/> event to be notified
    /// when the text recognition results change. 
    /// </summary>
    public sealed class TouchRecognizer : IDisposable
    {
        private readonly Ink _ink;
        private readonly Strokes _strokes;
        private readonly List<string> _recognitionResults;
        private TouchAdapter _inputAdapter;
        private RecognizerContext _recognizerContext;
        private bool _disposed;

        /// <summary>
        /// Event raised when the list of recognition results has changed.
        /// </summary>
        public event EventHandler<TouchRecognitionEventArgs> ResultsChanged;

        /// <summary>
        /// Initializes the touch recognizer. You must also set an
        /// input adapter with <see cref="SetAdapter(TouchAdapter)"/>
        /// and a text recognizer with <see cref="SetRecognizer(Recognizer)"/>.
        /// </summary>
        public TouchRecognizer()
        {
            _ink = new Ink();
            _strokes = _ink.Strokes;
            _recognitionResults = new List<string>();
        }
        
        /// <summary>
        /// Sets the stroke input source.
        /// </summary>
        /// <param name="newAdapter">The new input source.</param>
        public void SetAdapter(TouchAdapter newAdapter)
        {
            EnsureNotDisposed();
            TouchAdapter oldAdapter = _inputAdapter;
            if (oldAdapter != null)
            {
                oldAdapter.StrokeFinished -= OnStrokeFinished;
                oldAdapter.StrokesCleared -= OnStrokesCleared;
            }
            if (newAdapter != null)
            {
                newAdapter.StrokeFinished += OnStrokeFinished;
                newAdapter.StrokesCleared += OnStrokesCleared;
            }
            _inputAdapter = newAdapter;
            OnStrokesCleared(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the recognizer used to convert input to text.
        /// </summary>
        /// <param name="recognizer">The new text recognizer.</param>
        public void SetRecognizer(Recognizer recognizer)
        {
            EnsureNotDisposed();
            _recognizerContext?.Dispose();
            _recognizerContext = recognizer?.CreateRecognizerContext();
            RecognizeStrokes();
        }

        private void OnRecognitionResultsChanged()
        {
            ResultsChanged?.Invoke(this, new TouchRecognitionEventArgs(_recognitionResults));
        }

        private void RecognizeStrokes()
        {
            _recognitionResults.Clear();
            if (_recognizerContext == null)
            {
                Debug.WriteLine("No recognizer set, cannot recognize input");
                return;
            }
            if (_strokes.Count == 0) return;
            _recognizerContext.Strokes = _strokes;
            RecognitionStatus status;
            RecognitionResult recognizeResult = _recognizerContext.Recognize(out status);
            if (status != RecognitionStatus.NoError)
            {
                Debug.WriteLine("Text recognition failed with status: " + status);
                return;
            }
            RecognitionAlternates alternates = recognizeResult.GetAlternatesFromSelection();
            foreach (RecognitionAlternate alternate in alternates)
            {
                _recognitionResults.Add(alternate.ToString());
            }
            OnRecognitionResultsChanged();
        }

        private void OnStrokeFinished(object sender, TouchStrokeEventArgs e)
        {
            Stroke stroke = _ink.CreateStroke(e.Stroke.ToArray());
            _strokes.Add(stroke);
            RecognizeStrokes();
        }

        private void OnStrokesCleared(object sender, EventArgs e)
        {
            _strokes.Clear();
            _recognitionResults.Clear();
            OnRecognitionResultsChanged();
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _recognizerContext.Dispose();
            _strokes.Dispose();
            _ink.Dispose();
            _disposed = true;
        }
    }
}
